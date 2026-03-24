using MediatR;
using Microsoft.EntityFrameworkCore;
using PlovCenter.Application.Common.Interfaces.Contexts;
using PlovCenter.Application.Contract.Categories.Queries;
using PlovCenter.Application.Contract.Categories.Responses;

namespace PlovCenter.Application.Features.Categories.Queries;

public sealed class GetAdminCategoriesQueryHandler(IApplicationDbContext applicationDbContext)
    : IRequestHandler<GetAdminCategoriesQuery, IReadOnlyCollection<CategoryResponse>>
{
    public async Task<IReadOnlyCollection<CategoryResponse>> Handle(
        GetAdminCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        return await applicationDbContext.Categories
            .AsNoTracking()
            .OrderBy(static category => category.SortOrder)
            .ThenBy(static category => category.Name)
            .Select(category => new CategoryResponse(
                category.Id,
                category.Name,
                category.SortOrder,
                category.IsVisible,
                category.Dishes.Count(),
                category.CreatedUtc,
                category.UpdatedUtc))
            .ToArrayAsync(cancellationToken);
    }
}
