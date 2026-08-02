using Domain.Models;
using Web.Responses;

namespace Web.Extensions;

public static class CategoryExtensions
{
    public static CategoryResponse ToCategoryResponse(this Category category)
    {
        return new CategoryResponse(
            category.Id,
            category.Name,
            category.Description,
            category.IsActive
        );
    }
}