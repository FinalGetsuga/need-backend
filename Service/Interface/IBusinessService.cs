using Domain.DTO;
using Domain.Models;

namespace Service.Interface;

public interface IBusinessService
{
    Task<List<Business>> GetAllAsync();
    Task<List<Business>> GetAllByCategoryAsync(Guid categoryId);
    Task<Business> GetByIdAsync(Guid id);
    Task<Business> GetByIdNotNullAsync(Guid id);
    Task<Business> CreateAsync(string userId, BusinessDto dto);
    Task<Business> UpdateAsync(string userId, Guid id, BusinessDto dto);
    Task<Business> DeleteAsync(string userId, Guid id);
}