using MediatR;
using Microsoft.EntityFrameworkCore;
using PlovCenter.Application.Common.Interfaces.Contexts;
using PlovCenter.Application.Contract.Categories.Queries;
using PlovCenter.Application.Contract.Categories.Responses;
using PlovCenter.Application.Features.Categories.Mappings;

namespace PlovCenter.Application.Features.Categories.Queries;

public sealed class GetAdminCategoriesQueryHandler(IApplicationDbContext applicationDbContext)
    : IRequestHandler<GetAdminCategoriesQuery, IReadOnlyCollection<CategoryResponse>>
{
    public async Task<IReadOnlyCollection<CategoryResponse>> Handle(
        GetAdminCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var categories = await applicationDbContext.Categories
            .AsNoTracking()
            .Include(static category => category.Dishes)
            .OrderBy(static category => category.SortOrder)
            .ThenBy(static category => category.Name)
            .ToListAsync(cancellationToken);

        return categories.Select(static category => category.ToCategoryResponse()).ToArray();
    }
}
