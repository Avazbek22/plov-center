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
            .Include(static category => category.Dishes)
            .Where(category => category.IsVisible)
            .OrderBy(category => category.SortOrder)
            .ThenBy(category => category.Name)
            .ToListAsync(cancellationToken);

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
