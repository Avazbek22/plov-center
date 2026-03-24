using Microsoft.AspNetCore.Identity;
using PlovCenter.Application.Common.Interfaces.Services;
using PlovCenter.Domain.Entities;

namespace PlovCenter.Infrastructure.Authentication;

internal sealed class PasswordHashService : IPasswordHashService
{
    private readonly PasswordHasher<AdminUser> _passwordHasher = new();

    public string HashPassword(AdminUser adminUser, string password)
    {
        return _passwordHasher.HashPassword(adminUser, password);
    }

    public bool VerifyPassword(AdminUser adminUser, string password)
    {
        var result = _passwordHasher.VerifyHashedPassword(adminUser, adminUser.PasswordHash, password);
        return result is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
    }
}
