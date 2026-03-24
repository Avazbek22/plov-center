using PlovCenter.Domain.Common;

namespace PlovCenter.Domain.Entities;

public sealed class SiteContentEntry : AuditableEntity
{
    private SiteContentEntry()
    {
    }

    public SiteContentEntry(string key, string? value, DateTime utcNow)
    {
        InitializeAudit(utcNow);
        Key = NormalizeKey(key);
        Value = NormalizeValue(value);
    }

    public string Key { get; private set; } = string.Empty;

    public string? Value { get; private set; }

    public void UpdateValue(string? value, DateTime utcNow)
    {
        Value = NormalizeValue(value);
        Touch(utcNow);
    }

    private static string NormalizeKey(string value)
    {
        return value.Trim();
    }

    private static string? NormalizeValue(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
