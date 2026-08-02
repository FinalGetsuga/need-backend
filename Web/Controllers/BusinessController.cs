using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Auth;
using Web.Mappers;
using Web.Requests;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BusinessController : ControllerBase
{
    private readonly BusinessMapper _businessMapper;
    private readonly EmployeeMapper _employeeMapper;
    private readonly ICurrentUser _currentUser;

    public BusinessController(BusinessMapper businessMapper, EmployeeMapper employeeMapper, ICurrentUser currentUser)
    {
        _businessMapper = businessMapper;
        _employeeMapper = employeeMapper;
        _currentUser = currentUser;
    }

    // Everyone can see all the businesses.
    [HttpGet]
    public async Task<IActionResult> GetBusinessesAsync()
    {
        var result = await _businessMapper.GetAllAsync();
        return Ok(result);
    }
    
    // Admin and Owner can see the employees per business - Correct
    [Authorize(Roles = "Admin,Owner")]
    [HttpGet("{id}/employees")]
    public async Task<IActionResult> GetBusinessEmployeesAsync([FromRoute] Guid id)
    { 
        var result = await _employeeMapper.GetAllByBusinessAsync(_currentUser.UserId!, id);
        return Ok(result);
    }

    // Everyone has access to a certain business.
    [HttpGet("{id}")]
    public async Task<IActionResult> GetBusinessAsync([FromRoute] Guid id)
    {
        var result = await _businessMapper.GetByIdNotNullAsync(id);
        return Ok(result);
    }
    
    // Everyone can register a business if he is not already owner of one or an employee.
    [HttpPost("register")]
    public async Task<IActionResult> RegisterBusiness([FromBody] BusinessRequest request)
    {
        var result = await _businessMapper.CreateBusinessAsync(_currentUser.UserId!, request);
        return Ok(result);
    }

    // Admin and Owner can edit business. Owner his own, Admin every.
    [Authorize(Roles = "Admin,Owner")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBusinessAsync([FromRoute] Guid id, [FromBody] BusinessRequest request)
    {
        var result = await _businessMapper.UpdateAsync(_currentUser.UserId!, id, request);
        return Ok(result);
    }

    // Admin and Owner can delete business. Owner his own, Admin every.
    [Authorize(Roles = "Admin,Owner")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBusinessAsync([FromRoute] Guid id)
    {
        var result = await _businessMapper.DeleteAsync(_currentUser.UserId!, id);
        return Ok(result);
    }
}