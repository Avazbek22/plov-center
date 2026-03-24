using PlovCenter.Application.Common.Models;
using PlovCenter.Domain.Entities;

namespace PlovCenter.Application.Common.Interfaces.Services;

public interface IJwtTokenService
{
    JwtTokenResult Create(AdminUser adminUser);
}
