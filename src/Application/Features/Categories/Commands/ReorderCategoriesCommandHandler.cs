using MediatR;
using Microsoft.EntityFrameworkCore;
using PlovCenter.Application.Common.Exceptions;
using PlovCenter.Application.Common.Interfaces.Contexts;
using PlovCenter.Application.Common.Interfaces.Services;
using PlovCenter.Application.Contract.Categories.Commands;

namespace PlovCenter.Application.Features.Categories.Commands;

public sealed class ReorderCategoriesCommandHandler(
    IApplicationDbContext applicationDbContext,
    IDateTimeService dateTimeService) : IRequestHandler<ReorderCategoriesCommand, Unit>
{
    public async Task<Unit> Handle(ReorderCategoriesCommand request, CancellationToken cancellationToken)
    {
        var requestedIds = request.Items.Select(static item => item.CategoryId).ToArray();

        if (requestedIds.Distinct().Count() != requestedIds.Length)
        {
            throw new ConflictException("Category reorder payload contains duplicate identifiers.");
        }

        var categories = await applicationDbContext.Categories
            .Where(category => requestedIds.Contains(category.Id))
            .ToListAsync(cancellationToken);

        if (categories.Count != requestedIds.Length)
        {
            throw new NotFoundException("One or more categories were not found.");
        }

        var sortOrderById = request.Items.ToDictionary(static item => item.CategoryId, static item => item.SortOrder);
        var utcNow = dateTimeService.UtcNow;

        foreach (var category in categories)
        {
            category.SortOrder = sortOrderById[category.Id];
            category.UpdatedUtc = utcNow;
        }

        await applicationDbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
