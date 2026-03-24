using PlovCenter.Domain.Common;

namespace PlovCenter.Domain.Entities;

public sealed class AdminUser : AuditableEntity
{
    public string Username { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}
