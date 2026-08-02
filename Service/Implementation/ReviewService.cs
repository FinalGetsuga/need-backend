using Domain.DTO;
using Domain.Enums;
using Domain.Identity;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class ReviewService : IReviewService
{
    private readonly IRepository<Review> _repository;
    private readonly IRepository<Booking> _bookingRepository;
    private readonly UserManager<AppUser> _userManager;

    public ReviewService(IRepository<Review> repository, IRepository<Booking> bookingRepository, UserManager<AppUser> userManager)
    {
        _repository = repository;
        _bookingRepository = bookingRepository;
        _userManager = userManager;
    }

    public async Task<Review> GetByIdAsync(Guid id)
    {
        return await _repository.GetAsync(
            selector: x => x,
            predicate: x => x.Id == id);
    }

    public async Task<Review> GetByIdNotNullAsync(Guid id)
    {
        var entity = await this.GetByIdAsync(id);
        
        if (entity == null)
            throw new InvalidOperationException("Not found.");
        
        return entity;
    }

    public async Task<List<Review>> GetAllByBusinessAsync(Guid businessId)
    {
        return await _repository.GetAllAsync(
            selector: x => x,
            predicate: x => x.Booking.Term.BusinessId == businessId);
    }

    public async Task<List<Review>> GetAllByUserAsync(string userId)
    {
        return await _repository.GetAllAsync(
            selector: x => x,
            predicate: x => x.Booking.CustomerId == userId);
    }

    public async Task<Review> CreateAsync(string userId, CreateReviewDto dto)
    {
        var booking = await _bookingRepository.GetAsync(selector: x => x, predicate: x => x.Id == dto.BookingId);
        
        if (booking == null)
            throw new InvalidOperationException("Not found.");

        if (booking.CustomerId != userId)
            throw new UnauthorizedAccessException("You can only review your own bookings.");

        if (booking.Status != BookingStatus.Completed)
            throw new InvalidOperationException("You can only review a completed booking.");

        var alreadyReviewed = await _repository.ExistsAsync(x => x.BookingId == dto.BookingId);
        if (alreadyReviewed)
            throw new InvalidOperationException("This booking has already been reviewed.");
        
        ValidateRating(dto.Rating);

        var review = new Review
        {
            BookingId = dto.BookingId,
            Rating = dto.Rating,
            Comment = dto.Comment
        };

        return await _repository.InsertAsync(review);
    }

    public async Task<Review> UpdateAsync(string userId, Guid id, UpdateReviewDto dto)
    {
        var review = await this.GetByIdNotNullAsync(id);

        var booking = await _bookingRepository.GetAsync(selector: x => x, predicate: x => x.Id == review.BookingId);
        if (booking == null || booking.CustomerId != userId)
            throw new UnauthorizedAccessException("You can only edit your own review.");
        
        ValidateRating(dto.Rating);
        
        review.Rating = dto.Rating;
        review.Comment = dto.Comment;
        
        return await _repository.UpdateAsync(review);
    }

    public async Task<Review> DeleteAsync(string userId, Guid id)
    {
        var review = await this.GetByIdNotNullAsync(id);
        
        var user = await _userManager.FindByIdAsync(userId);
        bool isAdmin = user != null && await _userManager.IsInRoleAsync(user, "Admin");

        if (!isAdmin)
        {
            var booking = await _bookingRepository.GetAsync(selector: x => x, predicate: x => x.Id == review.BookingId);
            if (booking == null || booking.CustomerId != userId)
                throw new UnauthorizedAccessException("You can only delete your own review.");
        }
        return await _repository.DeleteAsync(review);
    }

    private static void ValidateRating(int rating)
    {
        if (rating < 1 || rating > 5)
            throw new InvalidOperationException("Rating must be between 1 and 5.");
    }
}