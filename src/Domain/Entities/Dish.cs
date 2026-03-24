using PlovCenter.Domain.Common;

namespace PlovCenter.Domain.Entities;

public sealed class Dish : AuditableEntity
{
    private Dish()
    {
    }

    public Dish(
        Guid categoryId,
        string name,
        string? description,
        decimal price,
        string? photoPath,
        int sortOrder,
        bool isVisible,
        DateTime utcNow)
    {
        InitializeAudit(utcNow);
        CategoryId = categoryId;
        Name = NormalizeName(name);
        Description = NormalizeNullable(description);
        Price = price;
        PhotoPath = NormalizeNullable(photoPath);
        SortOrder = sortOrder;
        IsVisible = isVisible;
    }

    public Guid CategoryId { get; private set; }

    public Category? Category { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public decimal Price { get; private set; }

    public string? PhotoPath { get; private set; }

    public int SortOrder { get; private set; }

    public bool IsVisible { get; private set; }

    public void Update(
        Guid categoryId,
        string name,
        string? description,
        decimal price,
        string? photoPath,
        int sortOrder,
        bool isVisible,
        DateTime utcNow)
    {
        CategoryId = categoryId;
        Name = NormalizeName(name);
        Description = NormalizeNullable(description);
        Price = price;
        PhotoPath = NormalizeNullable(photoPath);
        SortOrder = sortOrder;
        IsVisible = isVisible;
        Touch(utcNow);
    }

    public void SetVisibility(bool isVisible, DateTime utcNow)
    {
        IsVisible = isVisible;
        Touch(utcNow);
    }

    private static string NormalizeName(string value)
    {
        return value.Trim();
    }

    private static string? NormalizeNullable(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
