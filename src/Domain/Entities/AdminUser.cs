using PlovCenter.Domain.Common;

namespace PlovCenter.Domain.Entities;

public sealed class AdminUser : AuditableEntity
{
    private AdminUser()
    {
    }

    public AdminUser(string username, string passwordHash, bool isActive, DateTime utcNow)
    {
        InitializeAudit(utcNow);
        Username = NormalizeUsername(username);
        PasswordHash = passwordHash;
        IsActive = isActive;
    }

    public string Username { get; private set; } = string.Empty;

    public string PasswordHash { get; private set; } = string.Empty;

    public bool IsActive { get; private set; }

    public void UpdatePasswordHash(string passwordHash, DateTime utcNow)
    {
        PasswordHash = passwordHash;
        Touch(utcNow);
    }

    public void SetActive(bool isActive, DateTime utcNow)
    {
        IsActive = isActive;
        Touch(utcNow);
    }

    public void Rename(string username, DateTime utcNow)
    {
        Username = NormalizeUsername(username);
        Touch(utcNow);
    }

    private static string NormalizeUsername(string value)
    {
        return value.Trim().ToLowerInvariant();
    }
}
