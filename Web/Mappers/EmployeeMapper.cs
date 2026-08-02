using Domain.DTO;
using Service.Interface;
using Web.Extensions;
using Web.Requests;
using Web.Responses;

namespace Web.Mappers;

public class EmployeeMapper
{
    private readonly IEmployeeService _service;

    public EmployeeMapper(IEmployeeService service)
    {
        _service = service;
    }
    
    public async Task<List<EmployeeResponse>> GetAllAsync()
    {
        var employees = await _service.GetAllAsync();
        return employees.Select(c => c.ToEmployeeResponse()).ToList();
    }
    
    public async Task<List<EmployeeResponse>> GetAllByBusinessAsync(string userId, Guid businessId)
    {
        var employees = await _service.GetAllByBusinessAsync(userId, businessId);
        return employees.Select(c => c.ToEmployeeResponse()).ToList();
    }

    public async Task<EmployeeResponse> GetByIdNotNullAsync(Guid id)
    {
        var employee = await _service.GetByIdNotNullAsync(id);
        return employee.ToEmployeeResponse();
    }

    public async Task<EmployeeResponse> CreateAsync(string userId, EmployeeRequest request)
    {
        var dto = new EmployeeDto
        {
            BusinessId = request.BusinessId,
            UserId = request.UserId,
            IsActive = request.IsActive
        };
        
        var employee = await _service.CreateAsync(userId, dto);
        return employee.ToEmployeeResponse();
    }

    public async Task<EmployeeResponse> UpdateAsync(string userId, Guid id, bool isActive)
    {
        var employee = await _service.UpdateAsync(userId, id, isActive);
        return employee.ToEmployeeResponse();
    }

    public async Task<EmployeeResponse> DeleteAsync(string userId, Guid id)
    {
        var employee = await _service.DeleteAsync(userId, id);
        return employee.ToEmployeeResponse();
    }
}