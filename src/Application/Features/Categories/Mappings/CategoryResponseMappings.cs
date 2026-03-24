using PlovCenter.Application.Contract.Categories.Responses;
using PlovCenter.Domain.Entities;

namespace PlovCenter.Application.Features.Categories.Mappings;

internal static class CategoryResponseMappings
{
    public static CategoryResponse ToCategoryResponse(this Category category)
    {
        return new CategoryResponse(
            category.Id,
            category.Name,
            category.SortOrder,
            category.IsVisible,
            category.Dishes.Count,
            category.CreatedUtc,
            category.UpdatedUtc);
    }
}
