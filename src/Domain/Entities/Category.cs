using PlovCenter.Domain.Common;

namespace PlovCenter.Domain.Entities;

public sealed class Category : AuditableEntity
{
    private readonly List<Dish> _dishes = [];

    private Category()
    {
    }

    public Category(string name, int sortOrder, bool isVisible, DateTime utcNow)
    {
        InitializeAudit(utcNow);
        Name = NormalizeName(name);
        SortOrder = sortOrder;
        IsVisible = isVisible;
    }

    public string Name { get; private set; } = string.Empty;

    public int SortOrder { get; private set; }

    public bool IsVisible { get; private set; }

    public IReadOnlyCollection<Dish> Dishes => _dishes;

    public void Update(string name, int sortOrder, bool isVisible, DateTime utcNow)
    {
        Name = NormalizeName(name);
        SortOrder = sortOrder;
        IsVisible = isVisible;
        Touch(utcNow);
    }

    public void SetVisibility(bool isVisible, DateTime utcNow)
    {
        IsVisible = isVisible;
        Touch(utcNow);
    }

    public void SetSortOrder(int sortOrder, DateTime utcNow)
    {
        SortOrder = sortOrder;
        Touch(utcNow);
    }

    private static string NormalizeName(string value)
    {
        return value.Trim();
    }
}
