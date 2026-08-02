using Domain.DTO;
using Service.Interface;
using Web.Extensions;
using Web.Requests;
using Web.Responses;

namespace Web.Mappers;

public class CategoryMapper
{
    private readonly ICategoryService _service;

    public CategoryMapper(ICategoryService service)
    {
        _service = service;
    }
    
    public async Task<List<CategoryResponse>> GetAllAsync()
    {
        var categories = await _service.GetAllAsync();
        return categories.Select(c => c.ToCategoryResponse()).ToList();
    }

    public async Task<CategoryResponse> GetByIdNotNullAsync(Guid id)
    {
        var category = await _service.GetByIdNotNullAsync(id);
        return category.ToCategoryResponse();
    }

    public async Task<CategoryResponse> CreateAsync(CategoryRequest request)
    {
        var dto = new CategoryDto
        {
            Name = request.Name,
            Description = request.Description,
            IsActive = request.IsActive
        };
        
        var category = await _service.CreateAsync(dto);
        return category.ToCategoryResponse();
    }

    public async Task<CategoryResponse> UpdateAsync(Guid id, CategoryRequest request)
    {
        var dto = new CategoryDto
        {
            Name = request.Name,
            Description = request.Description,
            IsActive = request.IsActive
        };

        var category = await _service.UpdateAsync(id, dto);
        return category.ToCategoryResponse();
    }

    public async Task<CategoryResponse> DeleteAsync(Guid id)
    {
        var category = await _service.DeleteAsync(id);
        return category.ToCategoryResponse();
    }
}