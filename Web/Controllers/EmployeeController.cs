using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Auth;
using Web.Mappers;
using Web.Requests;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeeController : ControllerBase
{
    private readonly EmployeeMapper _mapper;
    private readonly ICurrentUser _currentUser;

    public EmployeeController(EmployeeMapper mapper, ICurrentUser currentUser)
    {
        _mapper = mapper;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllEmployees()
    {
        var result = await _mapper.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Owner")]
    public async Task<IActionResult> GetEmployee([FromRoute] Guid id)
    {
        var result = await _mapper.GetByIdNotNullAsync(id);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Owner")]
    public async Task<IActionResult> CreateEmployee([FromBody] EmployeeRequest request)
    {
        var result = await _mapper.CreateAsync(_currentUser.UserId!, request);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEmployee([FromRoute] Guid id, bool isActive)
    {
        var result = await _mapper.UpdateAsync(_currentUser.UserId!, id, isActive);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Owner")]
    public async Task<IActionResult> DeleteEmployee([FromRoute] Guid id)
    {
        var result = await _mapper.DeleteAsync(_currentUser.UserId!, id);
        return Ok(result);
    }
}