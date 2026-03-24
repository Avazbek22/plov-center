using MediatR;
using PlovCenter.Application.Common.Extensions;
using PlovCenter.Application.Common.Interfaces.Contexts;
using PlovCenter.Application.Common.Interfaces.Services;
using PlovCenter.Application.Contract.Categories.Commands;
using PlovCenter.Application.Contract.Categories.Responses;
using PlovCenter.Application.Features.Categories.Mappings;
using PlovCenter.Domain.Entities;

namespace PlovCenter.Application.Features.Categories.Commands;

public sealed class CreateCategoryCommandHandler(
    IApplicationDbContext applicationDbContext,
    IDateTimeService dateTimeService) : IRequestHandler<CreateCategoryCommand, CategoryResponse>
{
    public async Task<CategoryResponse> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var utcNow = dateTimeService.UtcNow;

        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = request.Name.NormalizeTrimmed(),
            SortOrder = request.SortOrder,
            IsVisible = request.IsVisible,
            CreatedUtc = utcNow,
            UpdatedUtc = utcNow
        };

        applicationDbContext.Categories.Add(category);
        await applicationDbContext.SaveChangesAsync(cancellationToken);

        return category.ToCategoryResponse(0);
    }
}
