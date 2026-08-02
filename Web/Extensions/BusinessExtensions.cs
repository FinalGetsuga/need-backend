using Domain.Models;
using Web.Responses;

namespace Web.Extensions;

public static class BusinessExtensions
{
    public static BusinessResponse ToBusinessResponse(this Business business)
    {
        return new BusinessResponse(
            business.Id,
            business.Name,
            business.Description,
            business.Address,
            business.ImageUrl,
            business.WebsiteUrl,
            business.CategoryId
        );
    }
}