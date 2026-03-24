using PlovCenter.Domain.Entities;

namespace PlovCenter.Application.Common.Interfaces.Services;

public interface IPasswordHashService
{
    string HashPassword(AdminUser adminUser, string password);

    bool VerifyPassword(AdminUser adminUser, string password);
}
