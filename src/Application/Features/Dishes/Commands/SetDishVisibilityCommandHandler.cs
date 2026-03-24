using MediatR;
using Microsoft.EntityFrameworkCore;
using PlovCenter.Application.Common.Exceptions;
using PlovCenter.Application.Common.Interfaces.Contexts;
using PlovCenter.Application.Common.Interfaces.Services;
using PlovCenter.Application.Contract.Dishes.Commands;
using PlovCenter.Application.Contract.Dishes.Responses;
using PlovCenter.Application.Features.Dishes.Mappings;

namespace PlovCenter.Application.Features.Dishes.Commands;

public sealed class SetDishVisibilityCommandHandler(
    IApplicationDbContext applicationDbContext,
    IDateTimeService dateTimeService) : IRequestHandler<SetDishVisibilityCommand, DishResponse>
{
    public async Task<DishResponse> Handle(SetDishVisibilityCommand request, CancellationToken cancellationToken)
    {
        var dish = await applicationDbContext.Dishes
            .Include(static item => item.Category)
            .FirstOrDefaultAsync(item => item.Id == request.DishId, cancellationToken)
            ?? throw new NotFoundException("Dish was not found.");

        dish.IsVisible = request.IsVisible;
        dish.UpdatedUtc = dateTimeService.UtcNow;

        await applicationDbContext.SaveChangesAsync(cancellationToken);

        return dish.ToDishResponse();
    }
}
