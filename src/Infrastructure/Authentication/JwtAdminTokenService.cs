using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PlovCenter.Application.Abstractions.Auth;
using PlovCenter.Domain.Entities;
using PlovCenter.Infrastructure.Configuration;

namespace PlovCenter.Infrastructure.Authentication;

internal sealed class JwtAdminTokenService(IOptions<JwtOptions> jwtOptions) : IAdminTokenService
{
    public AccessTokenResult CreateToken(AdminUser adminUser)
    {
        var options = jwtOptions.Value;
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(options.ExpiresMinutes);
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SigningKey));

        Claim[] claims =
        [
            new Claim(JwtRegisteredClaimNames.Sub, adminUser.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, adminUser.Id.ToString()),
            new Claim(ClaimTypes.Name, adminUser.Username),
            new Claim(JwtRegisteredClaimNames.UniqueName, adminUser.Username)
        ];

        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: options.Issuer,
            audience: options.Audience,
            claims: claims,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        return new AccessTokenResult(new JwtSecurityTokenHandler().WriteToken(token), expiresAtUtc);
    }
}
