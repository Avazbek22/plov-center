using MediatR;
using Microsoft.EntityFrameworkCore;
using PlovCenter.Application.Common.Exceptions;
using PlovCenter.Application.Common.Interfaces.Contexts;
using PlovCenter.Application.Contract.Categories.Queries;
using PlovCenter.Application.Contract.Categories.Responses;
using PlovCenter.Application.Features.Categories.Mappings;

namespace PlovCenter.Application.Features.Categories.Queries;

public sealed class GetCategoryByIdQueryHandler(IApplicationDbContext applicationDbContext)
    : IRequestHandler<GetCategoryByIdQuery, CategoryResponse>
{
    public async Task<CategoryResponse> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await applicationDbContext.Categories
            .AsNoTracking()
            .Include(static item => item.Dishes)
            .FirstOrDefaultAsync(item => item.Id == request.CategoryId, cancellationToken)
            ?? throw new NotFoundException("Category was not found.");

        return category.ToCategoryResponse();
    }
}
