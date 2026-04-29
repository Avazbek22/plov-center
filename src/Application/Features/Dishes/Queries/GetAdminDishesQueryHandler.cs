using MediatR;
using Microsoft.EntityFrameworkCore;
using PlovCenter.Application.Common.Interfaces.Contexts;
using PlovCenter.Application.Contract.Dishes.Queries;
using PlovCenter.Application.Contract.Dishes.Responses;

namespace PlovCenter.Application.Features.Dishes.Queries;

public sealed class GetAdminDishesQueryHandler(IApplicationDbContext applicationDbContext)
    : IRequestHandler<GetAdminDishesQuery, IReadOnlyCollection<DishResponse>>
{
    public async Task<IReadOnlyCollection<DishResponse>> Handle(GetAdminDishesQuery request, CancellationToken cancellationToken)
    {
        var query = applicationDbContext.Dishes
            .AsNoTracking()
            .AsQueryable();

        if (request.CategoryId.HasValue)
        {
            query = query.Where(item => item.CategoryId == request.CategoryId.Value);
        }

        return await query
            .OrderBy(item => item.Category!.SortOrder)
            .ThenBy(item => item.SortOrder)
            .ThenBy(item => item.Name)
            .Select(dish => new DishResponse(
                dish.Id,
                dish.CategoryId,
                dish.Category!.Name,
                dish.Name,
                dish.Description,
                dish.Price,
                dish.Photos
                    .OrderBy(photo => photo.SortOrder)
                    .Select(photo => new DishPhotoResponse(photo.Id, photo.RelativePath, photo.SortOrder))
                    .ToArray(),
                dish.SortOrder,
                dish.IsVisible,
                dish.CreatedUtc,
                dish.UpdatedUtc))
            .ToArrayAsync(cancellationToken);
    }
}
