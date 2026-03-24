using MediatR;
using Microsoft.EntityFrameworkCore;
using PlovCenter.Application.Common.Exceptions;
using PlovCenter.Application.Common.Interfaces.Contexts;
using PlovCenter.Application.Contract.Dishes.Queries;
using PlovCenter.Application.Contract.Dishes.Responses;
using PlovCenter.Application.Features.Dishes.Mappings;

namespace PlovCenter.Application.Features.Dishes.Queries;

public sealed class GetDishByIdQueryHandler(IApplicationDbContext applicationDbContext)
    : IRequestHandler<GetDishByIdQuery, DishResponse>
{
    public async Task<DishResponse> Handle(GetDishByIdQuery request, CancellationToken cancellationToken)
    {
        var dish = await applicationDbContext.Dishes
            .AsNoTracking()
            .Include(static item => item.Category)
            .FirstOrDefaultAsync(item => item.Id == request.DishId, cancellationToken)
            ?? throw new NotFoundException("Dish was not found.");

        return dish.ToDishResponse();
    }
}
