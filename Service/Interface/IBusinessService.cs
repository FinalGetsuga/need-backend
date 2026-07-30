using Domain.DTO;
using Domain.Models;

namespace Service.Interface;

public interface IBusinessService
{
    Task<List<Business>> GetAllAsync();
    Task<Business> GetByIdAsync(Guid id);
    Task<Business> GetByIdNotNullAsync(Guid id);
    Task<Business> CreateAsync(BusinessDto dto);
    Task<Business> UpdateAsync(Guid id, BusinessDto dto);
    Task<Business> DeleteAsync(Guid id);
}