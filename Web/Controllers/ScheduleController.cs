using Microsoft.AspNetCore.Mvc;
using Web.Auth;
using Web.Mappers;
using Web.Requests;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScheduleController : ControllerBase
{
    private readonly WorkScheduleMapper _mapper;
    private readonly ICurrentUser _currentUser;

    public ScheduleController(WorkScheduleMapper mapper, ICurrentUser currentUser)
    {
        _mapper = mapper;
        _currentUser = currentUser;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSchedule(Guid id)
    {
        var result = await _mapper.GetByBusinessIdAsync(id);
        return Ok(result);
    }
    
    [HttpPost("create")]
    public async Task<IActionResult> CreateSchedule(WorkScheduleRequest request)
    {
        var result = await _mapper.CreateAsync(_currentUser.UserId!, request);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSchedule(Guid id, WorkScheduleRequest request)
    {
        var result = await _mapper.UpdateAsync(_currentUser.UserId!, id, request);
        return Ok(result);
    }
}