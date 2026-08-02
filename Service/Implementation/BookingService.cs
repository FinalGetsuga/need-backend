using Domain.DTO;
using Domain.Enums;
using Domain.Identity;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class BookingService : IBookingService
{
    private readonly IRepository<Booking> _repository;
    private readonly IRepository<Term> _termRepository;
    private readonly IRepository<Business> _businessRepository;
    private readonly UserManager<AppUser> _userManager;

    public BookingService(IRepository<Booking> repository, IRepository<Term> termRepository, IRepository<Business> businessRepository,  UserManager<AppUser> userManager)
    {
        _repository = repository;
        _termRepository = termRepository;
        _businessRepository = businessRepository;
        _userManager = userManager;
    }

    public async Task<Booking> GetByIdAsync(Guid id)
    {
        return await _repository.GetAsync(
            selector: x => x,
            predicate: x => x.Id == id);
    }

    public async Task<Booking> GetByIdNotNullAsync(Guid id)
    {
        var entity = await this.GetByIdAsync(id);

        if (entity == null)
            throw new InvalidOperationException("Not found!");
        
        return entity;
    }

    public async Task<List<Booking>> GetAllByUserAsync(string userId)
    {
        return await _repository.GetAllAsync(
            selector: x => x,
            predicate: x => x.CustomerId == userId);
    }

    public async Task<List<Booking>> GetAllByBusinessAsync(string userId, Guid businessId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        bool isAdmin = user != null && await _userManager.IsInRoleAsync(user, "Admin");

        if (!isAdmin)
        {
            var isOwner = await _businessRepository.ExistsAsync(b => b.Id == businessId && b.OwnerId == userId);
            if (!isOwner)
                throw new UnauthorizedAccessException("Only the business owner can view its bookings");
        }
        
        return await _repository.GetAllAsync(
            selector: x => x,
            predicate: x => x.Term.BusinessId ==  businessId);
    }

    public async Task<Booking> CreateAsync(string userId, CreateBookingDto dto)
    {
        var term = await _termRepository.GetAsync(
            selector: x => x,
            predicate: x => x.Id == dto.TermId);

        if (term == null)
            throw new InvalidOperationException("Not found!");

        if (term.Status != TermStatus.Available)
            throw new InvalidOperationException("Not available!");

        var now = DateTime.UtcNow;
        var termStart = term.Date.ToDateTime(term.StartTime);
        if (termStart <= now)
            throw new InvalidOperationException("Cannot book a term in the past.");

        var booking = new Booking
        {
            TermId = term.Id,
            CustomerId = userId,
            Status = BookingStatus.Confirmed,
            Notes = dto.Notes,
            BookedAt = now
        };

        await _repository.InsertAsync(booking);

        term.Status = TermStatus.Booked;
        await _termRepository.UpdateAsync(term);

        return booking;
    }

    public async Task<Booking> CancelAsync(string userId, Guid id)
    {
        var booking = await this.GetByIdNotNullAsync(id);
        
        var user = await _userManager.FindByIdAsync(userId);
        bool isAdmin = user != null && await _userManager.IsInRoleAsync(user, "Admin");
        
        if (booking.Status != BookingStatus.Confirmed)
            throw new InvalidOperationException("Only a confirmed booking can be cancelled.");

        var term = await _termRepository.GetAsync(selector: x => x, predicate: x => x.Id == booking.TermId);
        if (term == null)
            throw new InvalidOperationException("Not found!");

        var isCustomer = booking.CustomerId == userId;
        var isOwner = !isCustomer &&
                      await _businessRepository.ExistsAsync(b => b.Id == term.BusinessId && b.OwnerId == userId);
        
        // treba i Admin
        if (!isOwner && !isCustomer && !isAdmin)
            throw new UnauthorizedAccessException("You are not allowed to cancel this booking.");

        booking.Status = isCustomer ? BookingStatus.CancelledByCustomer : BookingStatus.CancelledByBusiness;
        await _repository.UpdateAsync(booking);

        term.Status = TermStatus.Available;
        await _termRepository.UpdateAsync(term);

        return booking;
    }

    public async Task<Booking> DeleteAsync(string userId, Guid id)
    {
        var booking = await this.GetByIdNotNullAsync(id);

        var user = await _userManager.FindByIdAsync(userId);
        bool isAdmin = user != null && await _userManager.IsInRoleAsync(user, "Admin");

        if (!isAdmin && booking.CustomerId != userId)
            throw new UnauthorizedAccessException("You are not authorized to delete this booking.");

        if (booking.Status == BookingStatus.Confirmed)
            throw new InvalidOperationException("Cannot delete a confirmed booking - cancel it first.");

        return await _repository.DeleteAsync(booking);
    }

    public async Task MarkPastBookingsCompletedAsync()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var nowTime = TimeOnly.FromDateTime(DateTime.UtcNow);

        var pastBookings = await _repository.GetAllAsync(
            selector: x => x,
            predicate: b => b.Status == BookingStatus.Confirmed &&
                            (b.Term.Date < today || (b.Term.Date == today && b.Term.EndTime <= nowTime)));

        foreach (var booking in pastBookings)
        {
            booking.Status = BookingStatus.Completed;
            await _repository.UpdateAsync(booking);
        }
    }
}