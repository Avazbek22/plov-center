using MediatR;
using Microsoft.EntityFrameworkCore;
using PlovCenter.Application.Common.Interfaces.Contexts;
using PlovCenter.Application.Contract.Menu.Queries;
using PlovCenter.Application.Contract.Menu.Responses;

namespace PlovCenter.Application.Features.Menu.Queries;

public sealed class GetPublicMenuQueryHandler(IApplicationDbContext applicationDbContext)
    : IRequestHandler<GetPublicMenuQuery, PublicMenuResponse>
{
    public async Task<PublicMenuResponse> Handle(GetPublicMenuQuery request, CancellationToken cancellationToken)
    {
        var categories = await applicationDbContext.Categories
            .AsNoTracking()
            .Where(category => category.IsVisible)
            .OrderBy(category => category.SortOrder)
            .ThenBy(category => category.Name)
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
                        dish.Photos
                            .OrderBy(photo => photo.SortOrder)
                            .Select(photo => photo.RelativePath)
                            .ToArray(),
                        dish.SortOrder))
                    .ToArray()))
            .ToArrayAsync(cancellationToken);

        return new PublicMenuResponse(categories);
    }
}
