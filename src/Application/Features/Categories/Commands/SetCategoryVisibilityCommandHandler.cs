using MediatR;
using Microsoft.EntityFrameworkCore;
using PlovCenter.Application.Common.Exceptions;
using PlovCenter.Application.Common.Interfaces.Contexts;
using PlovCenter.Application.Common.Interfaces.Services;
using PlovCenter.Application.Contract.Categories.Commands;
using PlovCenter.Application.Contract.Categories.Responses;
using PlovCenter.Application.Features.Categories.Mappings;

namespace PlovCenter.Application.Features.Categories.Commands;

public sealed class SetCategoryVisibilityCommandHandler(
    IApplicationDbContext applicationDbContext,
    IDateTimeService dateTimeService) : IRequestHandler<SetCategoryVisibilityCommand, CategoryResponse>
{
    public async Task<CategoryResponse> Handle(SetCategoryVisibilityCommand request, CancellationToken cancellationToken)
    {
        var category = await applicationDbContext.Categories
            .FirstOrDefaultAsync(item => item.Id == request.CategoryId, cancellationToken)
            ?? throw new NotFoundException("Category was not found.");

        category.IsVisible = request.IsVisible;
        category.UpdatedUtc = dateTimeService.UtcNow;

        await applicationDbContext.SaveChangesAsync(cancellationToken);

        var dishCount = await applicationDbContext.Dishes
            .CountAsync(item => item.CategoryId == category.Id, cancellationToken);

        return category.ToCategoryResponse(dishCount);
    }
}
