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

        // RemoveRange must precede Clear: EF's change tracker keeps Deleted state on the entities even after the navigation collection is cleared.
        applicationDbContext.DishPhotos.RemoveRange(dish.Photos);
        dish.Photos.Clear();

        // Add via DbSet, not via dish.Photos.Add: attaching to a tracked parent's
        // navigation marks new entities with non-default Guid Ids as Unchanged,
        // which produces UPDATE statements that hit 0 rows on save. DbSet.Add
        // forces Added state → INSERT.
        foreach (var input in request.Photos.OrderBy(static p => p.SortOrder))
        {
            applicationDbContext.DishPhotos.Add(new DishPhoto
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
