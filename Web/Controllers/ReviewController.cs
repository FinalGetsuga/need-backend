using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Auth;
using Web.Mappers;
using Web.Requests;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReviewController : ControllerBase
{
    private readonly ReviewMapper _mapper;
    private readonly ICurrentUser _currentUser;

    public ReviewController(ReviewMapper mapper, ICurrentUser currentUser)
    {
        _mapper = mapper;
        _currentUser = currentUser;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetReviewById([FromRoute] Guid id)
    {
        var result = await _mapper.GetByIdNotNullAsync(id);
        return Ok(result);
    }

    [HttpGet("business/{id}")]
    public async Task<IActionResult> GetAllReviewsByBusiness([FromRoute] Guid id)
    {
        var result = await _mapper.GetAllByBusinessAsync(id);
        return Ok(result);
    }

    [HttpGet("mine")]
    public async Task<IActionResult> GetAllReviewsByUser()
    {
        var result = await _mapper.GetAllByUserAsync(_currentUser.UserId!);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateReview([FromBody] CreateReviewRequest request)
    {
        var result = await _mapper.CreateAsync(_currentUser.UserId!, request);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateReview([FromRoute] Guid id, [FromBody] UpdateReviewRequest request)
    {
        var result = await _mapper.UpdateAsync(_currentUser.UserId!, id, request);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReview([FromRoute] Guid id)
    {
        var result = await _mapper.DeleteAsync(_currentUser.UserId!, id);
        return Ok(result);
    }
}