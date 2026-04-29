using PlovCenter.Domain.Common;

namespace PlovCenter.Domain.Entities;

public sealed class DishPhoto : AuditableEntity
{
    public Guid DishId { get; set; }

    public Dish? Dish { get; set; }

    public string RelativePath { get; set; } = string.Empty;

    public int SortOrder { get; set; }
}
