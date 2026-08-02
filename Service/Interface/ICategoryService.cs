using Domain.DTO;
using Domain.Models;

namespace Service.Interface;

public interface ICategoryService
{
    Task<List<Category>> GetAllAsync();
    Task<Category> GetByIdAsync(Guid id);
    Task<Category> GetByIdNotNullAsync(Guid id);
    Task<Category> CreateAsync(CategoryDto dto);
    Task<Category> UpdateAsync(Guid id, CategoryDto dto);
    Task<Category> DeleteAsync(Guid id);
}