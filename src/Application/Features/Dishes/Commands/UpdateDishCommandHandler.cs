using MediatR;
using Microsoft.EntityFrameworkCore;
using PlovCenter.Application.Common.Extensions;
using PlovCenter.Application.Common.Exceptions;
using PlovCenter.Application.Common.Interfaces.Contexts;
using PlovCenter.Application.Common.Interfaces.Services;
using PlovCenter.Application.Contract.Dishes.Commands;
using PlovCenter.Application.Contract.Dishes.Responses;
using PlovCenter.Application.Features.Dishes.Mappings;

namespace PlovCenter.Application.Features.Dishes.Commands;

public sealed class UpdateDishCommandHandler(
    IApplicationDbContext applicationDbContext,
    IDateTimeService dateTimeService) : IRequestHandler<UpdateDishCommand, DishResponse>
{
    public async Task<DishResponse> Handle(UpdateDishCommand request, CancellationToken cancellationToken)
    {
        var dish = await applicationDbContext.Dishes
            .Include(static item => item.Category)
            .FirstOrDefaultAsync(item => item.Id == request.DishId, cancellationToken)
            ?? throw new NotFoundException("Dish was not found.");

        var category = await applicationDbContext.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == request.CategoryId, cancellationToken)
            ?? throw new NotFoundException("Category was not found.");

        dish.CategoryId = request.CategoryId;
        dish.Name = request.Name.NormalizeTrimmed();
        dish.Description = request.Description.NormalizeOptional();
        dish.Price = request.Price;
        dish.PhotoPath = request.PhotoPath.NormalizeOptional();
        dish.SortOrder = request.SortOrder;
        dish.IsVisible = request.IsVisible;
        dish.UpdatedUtc = dateTimeService.UtcNow;

        await applicationDbContext.SaveChangesAsync(cancellationToken);

        return dish.ToDishResponse(category.Name);
    }
}
