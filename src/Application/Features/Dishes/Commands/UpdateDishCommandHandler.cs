using MediatR;
using Microsoft.EntityFrameworkCore;
using PlovCenter.Application.Common.Extensions;
using PlovCenter.Application.Common.Exceptions;
using PlovCenter.Application.Common.Interfaces.Contexts;
using PlovCenter.Application.Common.Interfaces.Services;
using PlovCenter.Application.Contract.Dishes.Commands;
using PlovCenter.Application.Contract.Dishes.Responses;
using PlovCenter.Application.Features.Dishes.Mappings;
using PlovCenter.Domain.Entities;

namespace PlovCenter.Application.Features.Dishes.Commands;

public sealed class UpdateDishCommandHandler(
    IApplicationDbContext applicationDbContext,
    IDateTimeService dateTimeService) : IRequestHandler<UpdateDishCommand, DishResponse>
{
    public async Task<DishResponse> Handle(UpdateDishCommand request, CancellationToken cancellationToken)
    {
        var dish = await applicationDbContext.Dishes
            .Include(static item => item.Category)
            .Include(static item => item.Photos)
            .FirstOrDefaultAsync(item => item.Id == request.DishId, cancellationToken)
            ?? throw new NotFoundException("Dish was not found.");

        var category = await applicationDbContext.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == request.CategoryId, cancellationToken)
            ?? throw new NotFoundException("Category was not found.");

        var utcNow = dateTimeService.UtcNow;

        dish.CategoryId = request.CategoryId;
        dish.Name = request.Name.NormalizeTrimmed();
        dish.Description = request.Description.NormalizeOptional();
        dish.Price = request.Price;
        dish.SortOrder = request.SortOrder;
        dish.IsVisible = request.IsVisible;
        dish.UpdatedUtc = utcNow;

        applicationDbContext.DishPhotos.RemoveRange(dish.Photos);
        dish.Photos.Clear();

        foreach (var input in request.Photos.OrderBy(static p => p.SortOrder))
        {
            dish.Photos.Add(new DishPhoto
            {
                Id = Guid.NewGuid(),
                DishId = dish.Id,
                RelativePath = input.RelativePath.NormalizeTrimmed(),
                SortOrder = input.SortOrder,
                CreatedUtc = utcNow,
                UpdatedUtc = utcNow
            });
        }

        await applicationDbContext.SaveChangesAsync(cancellationToken);

        return dish.ToDishResponse(category.Name);
    }
}
