using PlovCenter.Domain.Entities;

namespace PlovCenter.Application.Abstractions.Persistence;

public interface IDishRepository
{
    Task<IReadOnlyList<Dish>> GetAllAsync(Guid? categoryId, CancellationToken cancellationToken);

    Task<Dish?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    void Add(Dish dish);

    void Remove(Dish dish);
}
