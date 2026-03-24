using MediatR;
using Microsoft.EntityFrameworkCore;
using PlovCenter.Application.Common.Exceptions;
using PlovCenter.Application.Common.Interfaces.Contexts;
using PlovCenter.Application.Contract.Categories.Commands;

namespace PlovCenter.Application.Features.Categories.Commands;

public sealed class DeleteCategoryCommandHandler(IApplicationDbContext applicationDbContext)
    : IRequestHandler<DeleteCategoryCommand, Unit>
{
    public async Task<Unit> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await applicationDbContext.Categories
            .FirstOrDefaultAsync(item => item.Id == request.CategoryId, cancellationToken)
            ?? throw new NotFoundException("Category was not found.");

        var hasDishes = await applicationDbContext.Dishes
            .AnyAsync(item => item.CategoryId == category.Id, cancellationToken);

        if (hasDishes)
        {
            throw new ConflictException("A category with dishes cannot be deleted.");
        }

        applicationDbContext.Categories.Remove(category);
        await applicationDbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
