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

public sealed class CreateDishCommandHandler(
    IApplicationDbContext applicationDbContext,
    IDateTimeService dateTimeService) : IRequestHandler<CreateDishCommand, DishResponse>
{
    public async Task<DishResponse> Handle(CreateDishCommand request, CancellationToken cancellationToken)
    {
        var category = await applicationDbContext.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == request.CategoryId, cancellationToken)
            ?? throw new NotFoundException("Category was not found.");

        var utcNow = dateTimeService.UtcNow;

        var dish = new Dish
        {
            Id = Guid.NewGuid(),
            CategoryId = request.CategoryId,
            Name = request.Name.NormalizeTrimmed(),
            Description = request.Description.NormalizeOptional(),
            Price = request.Price,
            SortOrder = request.SortOrder,
            IsVisible = request.IsVisible,
            CreatedUtc = utcNow,
            UpdatedUtc = utcNow
        };

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

        applicationDbContext.Dishes.Add(dish);
        await applicationDbContext.SaveChangesAsync(cancellationToken);

        return dish.ToDishResponse(category.Name);
    }
}
