using PlovCenter.Domain.Entities;

namespace PlovCenter.Application.Abstractions.Persistence;

public interface ICategoryRepository
{
    Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<Category>> GetByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken);

    Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Category?> GetByIdWithDishesAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<Category>> GetVisibleWithVisibleDishesAsync(CancellationToken cancellationToken);

    void Add(Category category);

    void Remove(Category category);
}
