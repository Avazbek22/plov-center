using MediatR;
using Microsoft.EntityFrameworkCore;
using PlovCenter.Application.Common.Exceptions;
using PlovCenter.Application.Common.Interfaces.Contexts;
using PlovCenter.Application.Contract.Dishes.Queries;
using PlovCenter.Application.Contract.Dishes.Responses;

namespace PlovCenter.Application.Features.Dishes.Queries;

public sealed class GetDishByIdQueryHandler(IApplicationDbContext applicationDbContext)
    : IRequestHandler<GetDishByIdQuery, DishResponse>
{
    public async Task<DishResponse> Handle(GetDishByIdQuery request, CancellationToken cancellationToken)
    {
        var dish = await applicationDbContext.Dishes
            .AsNoTracking()
            .Where(item => item.Id == request.DishId)
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
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException("Dish was not found.");

        return dish;
    }
}
