using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PlovCenter.Application.Common.Constants;
using PlovCenter.Application.Common.Interfaces.Services;
using PlovCenter.Application.Common.Models;
using PlovCenter.Domain.Entities;
using PlovCenter.Infrastructure.Configuration;

namespace PlovCenter.Infrastructure.Authentication;

internal sealed class JwtTokenService(IOptions<JwtOptions> jwtOptions) : IJwtTokenService
{
    public JwtTokenResult Create(AdminUser adminUser)
    {
        var options = jwtOptions.Value;
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(options.ExpiresMinutes);
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SigningKey));

        Claim[] claims =
        [
            new Claim(JwtRegisteredClaimNames.Sub, adminUser.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, adminUser.Id.ToString()),
            new Claim(ClaimTypes.Name, adminUser.Username),
            new Claim(JwtRegisteredClaimNames.UniqueName, adminUser.Username),
            new Claim(AdminClaimTypes.Access, bool.TrueString.ToLowerInvariant())
        ];

        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: options.Issuer,
            audience: options.Audience,
            claims: claims,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        return new JwtTokenResult(new JwtSecurityTokenHandler().WriteToken(token), expiresAtUtc);
    }
}
