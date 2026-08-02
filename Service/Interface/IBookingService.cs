using Domain.DTO;
using Domain.Models;

namespace Service.Interface;

public interface IBookingService
{
    Task<Booking> GetByIdAsync(Guid id);
    Task<Booking> GetByIdNotNullAsync(Guid id);
    Task<List<Booking>> GetAllByUserAsync(string userId);
    Task<List<Booking>> GetAllByBusinessAsync(string userId, Guid businessId);
    Task<Booking> CreateAsync(string userId, CreateBookingDto dto);
    Task<Booking> CancelAsync(string userId, Guid id);
    Task<Booking> DeleteAsync(string userId, Guid id);
    Task MarkPastBookingsCompletedAsync();
}