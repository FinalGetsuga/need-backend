using Domain.DTO;
using Domain.Models;

namespace Service.Interface;

public interface IReviewService
{
    Task<Review> GetByIdAsync(Guid id);
    Task<Review> GetByIdNotNullAsync(Guid id);
    Task<List<Review>> GetAllByBusinessAsync(Guid businessId);
    Task<List<Review>> GetAllByUserAsync(string userId);
    Task<Review> CreateAsync(string userId, CreateReviewDto dto);
    Task<Review> UpdateAsync(string userId, Guid id, UpdateReviewDto dto);
    Task<Review> DeleteAsync(string userId, Guid id);
}