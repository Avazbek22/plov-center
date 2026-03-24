using Microsoft.EntityFrameworkCore;
using PlovCenter.Application.Abstractions.Persistence;
using PlovCenter.Domain.Entities;

namespace PlovCenter.Infrastructure.Persistence.Repositories;

internal sealed class DishRepository(PlovCenterDbContext dbContext) : IDishRepository
{
    public async Task<IReadOnlyList<Dish>> GetAllAsync(Guid? categoryId, CancellationToken cancellationToken)
    {
        var query = dbContext.Dishes
            .Include(static dish => dish.Category)
            .AsQueryable();

        if (categoryId.HasValue)
        {
            query = query.Where(dish => dish.CategoryId == categoryId.Value);
        }

        return await query
            .OrderBy(static dish => dish.Category!.SortOrder)
            .ThenBy(static dish => dish.SortOrder)
            .ThenBy(static dish => dish.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<Dish?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.Dishes
            .Include(static dish => dish.Category)
            .FirstOrDefaultAsync(dish => dish.Id == id, cancellationToken);
    }

    public void Add(Dish dish)
    {
        dbContext.Dishes.Add(dish);
    }

    public void Remove(Dish dish)
    {
        dbContext.Dishes.Remove(dish);
    }
}
