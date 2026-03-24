using PlovCenter.Domain.Common;

namespace PlovCenter.Domain.Entities;

public sealed class SiteContentEntry : AuditableEntity
{
    public string Key { get; set; } = string.Empty;

    public string? Value { get; set; }
}
