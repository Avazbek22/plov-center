using PlovCenter.Application.Abstractions.Persistence;
using PlovCenter.Application.Common.Cqrs;
using PlovCenter.Application.Contract.Menu;

namespace PlovCenter.Application.Features.Menu;

public sealed record GetPublicMenuQuery() : IApplicationRequest<PublicMenuResponse>;

internal sealed class GetPublicMenuQueryHandler(ICategoryRepository categoryRepository)
    : IApplicationRequestHandler<GetPublicMenuQuery, PublicMenuResponse>
{
    public async Task<PublicMenuResponse> HandleAsync(GetPublicMenuQuery request, CancellationToken cancellationToken)
    {
        var categories = await categoryRepository.GetVisibleWithVisibleDishesAsync(cancellationToken);

        var payload = categories
            .Select(category => new PublicMenuCategoryResponse(
                category.Id,
                category.Name,
                category.SortOrder,
                category.Dishes
                    .Where(static dish => dish.IsVisible)
                    .OrderBy(static dish => dish.SortOrder)
                    .ThenBy(static dish => dish.Name)
                    .Select(dish => new PublicMenuDishResponse(
                        dish.Id,
                        dish.Name,
                        dish.Description,
                        dish.Price,
                        dish.PhotoPath,
                        dish.SortOrder))
                    .ToArray()))
            .ToArray();

        return new PublicMenuResponse(payload);
    }
}
