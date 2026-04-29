using PlovCenter.Domain.Common;

namespace PlovCenter.Domain.Entities;

public sealed class Dish : AuditableEntity
{
    public Guid CategoryId { get; set; }

    public Category? Category { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int SortOrder { get; set; }

    public bool IsVisible { get; set; }

    public List<DishPhoto> Photos { get; set; } = [];
}
