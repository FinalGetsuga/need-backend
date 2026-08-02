using Domain.DTO;
using Service.Interface;
using Web.Extensions;
using Web.Requests;
using Web.Responses;

namespace Web.Mappers;

public class BusinessMapper
{
    private readonly IBusinessService _service;

    public BusinessMapper(IBusinessService service)
    {
        _service = service;
    }
    
    public async Task<List<BusinessResponse>> GetAllAsync()
    {
        var businesses = await _service.GetAllAsync();
        return businesses.Select(b => b.ToBusinessResponse()).ToList();
    }

    public async Task<List<BusinessResponse>> GetAllByCategoryAsync(Guid categoryId)
    {
        var businesses = await _service.GetAllByCategoryAsync(categoryId);
        return businesses.Select(b => b.ToBusinessResponse()).ToList();
    }

    public async Task<BusinessResponse> GetByIdNotNullAsync(Guid id)
    {
        var business = await _service.GetByIdNotNullAsync(id);
        return business.ToBusinessResponse();
    }

    public async Task<BusinessResponse> CreateBusinessAsync(string userId, BusinessRequest businessRequest)
    {
        var dto = new BusinessDto
        {
            Name = businessRequest.Name,
            Description = businessRequest.Description,
            Address = businessRequest.Address,
            ImageUrl = businessRequest.ImageUrl,
            WebsiteUrl = businessRequest.WebsiteUrl,
            CategoryId = businessRequest.CategoryId
        };

        var result = await _service.CreateAsync(userId, dto);
        return result.ToBusinessResponse();
    }
    
    public async Task<BusinessResponse> UpdateAsync(string userId, Guid id, BusinessRequest request)
    {
        var dto = new BusinessDto
        {
            Name = request.Name,
            Description = request.Description,
            Address = request.Address,
            ImageUrl = request.ImageUrl,
            WebsiteUrl = request.WebsiteUrl,
            CategoryId = request.CategoryId
        };

        var result = await _service.UpdateAsync(userId, id, dto);
        return result.ToBusinessResponse();
    }

    public async Task<BusinessResponse> DeleteAsync(string userId, Guid id)
    {
        var result = await _service.DeleteAsync(userId, id);
        return result.ToBusinessResponse();
    }
}