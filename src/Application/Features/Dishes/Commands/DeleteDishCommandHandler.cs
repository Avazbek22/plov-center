using MediatR;
using Microsoft.EntityFrameworkCore;
using PlovCenter.Application.Common.Exceptions;
using PlovCenter.Application.Common.Interfaces.Contexts;
using PlovCenter.Application.Contract.Dishes.Commands;

namespace PlovCenter.Application.Features.Dishes.Commands;

public sealed class DeleteDishCommandHandler(IApplicationDbContext applicationDbContext)
    : IRequestHandler<DeleteDishCommand, Unit>
{
    public async Task<Unit> Handle(DeleteDishCommand request, CancellationToken cancellationToken)
    {
        var dish = await applicationDbContext.Dishes
            .FirstOrDefaultAsync(item => item.Id == request.DishId, cancellationToken)
            ?? throw new NotFoundException("Dish was not found.");

        applicationDbContext.Dishes.Remove(dish);
        await applicationDbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
