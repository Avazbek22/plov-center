using PlovCenter.Domain.Entities;

namespace PlovCenter.Application.Abstractions.Auth;

public interface IAdminTokenService
{
    AccessTokenResult CreateToken(AdminUser adminUser);
}
