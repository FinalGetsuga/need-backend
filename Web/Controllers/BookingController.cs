using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Auth;
using Web.Mappers;
using Web.Requests;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookingController : ControllerBase
{
    private readonly BookingMapper _mapper;
    private readonly ICurrentUser _currentUser;

    public BookingController(BookingMapper mapper, ICurrentUser currentUser)
    {
        _mapper = mapper;
        _currentUser = currentUser;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBookingById(Guid id)
    {
        var result = await _mapper.GetByIdNotNullAsync(id);
        return Ok(result);
    }

    [HttpGet("business/{id}")]
    public async Task<IActionResult> GetAllBookingsByBusiness(Guid id)
    {
        var result = await _mapper.GetAllByBusinessAsync(_currentUser.UserId!, id);
        return Ok(result);
    }

    [HttpGet("mine")]
    public async Task<IActionResult> GetAllBookingsByUser()
    {
        var result = await _mapper.GetAllByUserAsync(_currentUser.UserId!);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] BookingRequest request)
    {
        var result = await _mapper.CreateAsync(_currentUser.UserId!, request);
        return Ok(result);
    }

    [HttpPut("{id}/cancel")]
    public async Task<IActionResult> CancelBooking([FromRoute] Guid id)
    {
        var result = await _mapper.CancelAsync(_currentUser.UserId!, id);
        return Ok(result);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBooking([FromRoute] Guid id)
    {
        var result = await _mapper.DeleteAsync(_currentUser.UserId!, id);
        return Ok(result);
    }
}