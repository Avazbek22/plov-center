namespace PlovCenter.Infrastructure.Configuration;

public sealed class SeedAdminOptions
{
    public const string SectionName = "SeedAdmin";

    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}
