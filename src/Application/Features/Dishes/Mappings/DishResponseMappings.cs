using PlovCenter.Application.Contract.Dishes.Responses;
using PlovCenter.Domain.Entities;

namespace PlovCenter.Application.Features.Dishes.Mappings;

internal static class DishResponseMappings
{
    public static DishResponse ToDishResponse(this Dish dish, string? categoryName = null)
    {
        return new DishResponse(
            dish.Id,
            dish.CategoryId,
            categoryName ?? dish.Category?.Name ?? string.Empty,
            dish.Name,
            dish.Description,
            dish.Price,
            dish.PhotoPath,
            dish.SortOrder,
            dish.IsVisible,
            dish.CreatedUtc,
            dish.UpdatedUtc);
    }
}
