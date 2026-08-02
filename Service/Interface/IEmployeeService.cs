using Domain.DTO;
using Domain.Models;

namespace Service.Interface;

public interface IEmployeeService
{
    Task<List<Employee>> GetAllAsync();
    Task<List<Employee>> GetAllByBusinessAsync(string userId, Guid businessId);
    Task<Employee> GetByIdAsync(Guid id);
    Task<Employee> GetByIdNotNullAsync(Guid id);
    Task<Employee> CreateAsync(string userId, EmployeeDto dto);
    Task<Employee> UpdateAsync(string userId, Guid id, bool isActive);
    Task<Employee> DeleteAsync(string userId, Guid id);
}