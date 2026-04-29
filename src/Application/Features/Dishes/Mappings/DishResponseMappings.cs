using PlovCenter.Application.Contract.Dishes.Responses;
using PlovCenter.Domain.Entities;

namespace PlovCenter.Application.Features.Dishes.Mappings;

internal static class DishResponseMappings
{
    public static DishResponse ToDishResponse(this Dish dish, string? categoryName = null)
    {
        var photos = dish.Photos
            .OrderBy(static photo => photo.SortOrder)
            .Select(static photo => new DishPhotoResponse(photo.Id, photo.RelativePath, photo.SortOrder))
            .ToArray();

        return new DishResponse(
            dish.Id,
            dish.CategoryId,
            categoryName ?? dish.Category?.Name ?? string.Empty,
            dish.Name,
            dish.Description,
            dish.Price,
            photos,
            dish.SortOrder,
            dish.IsVisible,
            dish.CreatedUtc,
            dish.UpdatedUtc);
    }
}
