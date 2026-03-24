using PlovCenter.Application.Contract.Categories.Responses;
using PlovCenter.Domain.Entities;

namespace PlovCenter.Application.Features.Categories.Mappings;

internal static class CategoryResponseMappings
{
    public static CategoryResponse ToCategoryResponse(this Category category, int dishCount)
    {
        return new CategoryResponse(
            category.Id,
            category.Name,
            category.SortOrder,
            category.IsVisible,
            dishCount,
            category.CreatedUtc,
            category.UpdatedUtc);
    }

    public static CategoryResponse ToCategoryResponse(this Category category)
    {
        return category.ToCategoryResponse(category.Dishes.Count);
    }
}
