using MediatR;
using Microsoft.EntityFrameworkCore;
using PlovCenter.Application.Common.Interfaces.Contexts;
using PlovCenter.Application.Contract.Dishes.Queries;
using PlovCenter.Application.Contract.Dishes.Responses;
using PlovCenter.Application.Features.Dishes.Mappings;

namespace PlovCenter.Application.Features.Dishes.Queries;

public sealed class GetAdminDishesQueryHandler(IApplicationDbContext applicationDbContext)
    : IRequestHandler<GetAdminDishesQuery, IReadOnlyCollection<DishResponse>>
{
    public async Task<IReadOnlyCollection<DishResponse>> Handle(GetAdminDishesQuery request, CancellationToken cancellationToken)
    {
        var query = applicationDbContext.Dishes
            .AsNoTracking()
            .Include(static item => item.Category)
            .AsQueryable();

        if (request.CategoryId.HasValue)
        {
            query = query.Where(item => item.CategoryId == request.CategoryId.Value);
        }

        var dishes = await query
            .OrderBy(item => item.Category!.SortOrder)
            .ThenBy(item => item.SortOrder)
            .ThenBy(item => item.Name)
            .ToListAsync(cancellationToken);

        return dishes.Select(static dish => dish.ToDishResponse()).ToArray();
    }
}
