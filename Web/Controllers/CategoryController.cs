using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Mappers;
using Web.Requests;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly CategoryMapper _mapper;
    private readonly BusinessMapper _mapperBusiness;

    public CategoryController(CategoryMapper mapper, BusinessMapper mapperBusiness)
    {
        _mapper = mapper;
        _mapperBusiness = mapperBusiness;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCategories()
    {
        var result = await _mapper.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}/businesses")]
    public async Task<IActionResult> GetAllBusinessesByCategory(Guid categoryId)
    {
        var result = await _mapperBusiness.GetAllByCategoryAsync(categoryId);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategoryById([FromRoute] Guid id)
    {
        var result = await _mapper.GetByIdNotNullAsync(id);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateCategory([FromBody] CategoryRequest request)
    {
        var result = await _mapper.CreateAsync(request);
        return Ok(result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateCategory([FromRoute] Guid id, [FromBody] CategoryRequest request)
    {
        var result = await _mapper.UpdateAsync(id, request);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteCategory([FromRoute] Guid id)
    {
        var result = await _mapper.DeleteAsync(id);
        return Ok(result);
    }
}