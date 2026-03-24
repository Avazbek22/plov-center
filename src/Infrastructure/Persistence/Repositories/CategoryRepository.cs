using Microsoft.EntityFrameworkCore;
using PlovCenter.Application.Abstractions.Persistence;
using PlovCenter.Domain.Entities;

namespace PlovCenter.Infrastructure.Persistence.Repositories;

internal sealed class CategoryRepository(PlovCenterDbContext dbContext) : ICategoryRepository
{
    public async Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Categories
            .Include(static category => category.Dishes)
            .OrderBy(static category => category.SortOrder)
            .ThenBy(static category => category.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Category>> GetByIdsAsync(
        IReadOnlyCollection<Guid> ids,
        CancellationToken cancellationToken)
    {
        return await dbContext.Categories
            .Where(category => ids.Contains(category.Id))
            .ToListAsync(cancellationToken);
    }

    public Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.Categories.FirstOrDefaultAsync(category => category.Id == id, cancellationToken);
    }

    public Task<Category?> GetByIdWithDishesAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.Categories
            .Include(static category => category.Dishes)
            .FirstOrDefaultAsync(category => category.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Category>> GetVisibleWithVisibleDishesAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Categories
            .AsNoTracking()
            .Where(static category => category.IsVisible)
            .Include(category => category.Dishes.Where(dish => dish.IsVisible))
            .OrderBy(static category => category.SortOrder)
            .ThenBy(static category => category.Name)
            .ToListAsync(cancellationToken);
    }

    public void Add(Category category)
    {
        dbContext.Categories.Add(category);
    }

    public void Remove(Category category)
    {
        dbContext.Categories.Remove(category);
    }
}
