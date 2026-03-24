using MediatR;
using Microsoft.EntityFrameworkCore;
using PlovCenter.Application.Common.Exceptions;
using PlovCenter.Application.Common.Interfaces.Contexts;
using PlovCenter.Application.Contract.Categories.Queries;
using PlovCenter.Application.Contract.Categories.Responses;

namespace PlovCenter.Application.Features.Categories.Queries;

public sealed class GetCategoryByIdQueryHandler(IApplicationDbContext applicationDbContext)
    : IRequestHandler<GetCategoryByIdQuery, CategoryResponse>
{
    public async Task<CategoryResponse> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await applicationDbContext.Categories
            .AsNoTracking()
            .Where(item => item.Id == request.CategoryId)
            .Select(category => new CategoryResponse(
                category.Id,
                category.Name,
                category.SortOrder,
                category.IsVisible,
                category.Dishes.Count(),
                category.CreatedUtc,
                category.UpdatedUtc))
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException("Category was not found.");

        return category;
    }
}
