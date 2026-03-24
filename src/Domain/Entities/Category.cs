using PlovCenter.Domain.Common;

namespace PlovCenter.Domain.Entities;

public sealed class Category : AuditableEntity
{
    public string Name { get; set; } = string.Empty;

    public int SortOrder { get; set; }

    public bool IsVisible { get; set; }

    public ICollection<Dish> Dishes { get; set; } = new List<Dish>();
}
