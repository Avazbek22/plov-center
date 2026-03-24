using PlovCenter.Domain.Entities;

namespace PlovCenter.Application.Abstractions.Auth;

public interface IPasswordHashService
{
    string HashPassword(AdminUser adminUser, string password);

    bool VerifyPassword(AdminUser adminUser, string password);
}
