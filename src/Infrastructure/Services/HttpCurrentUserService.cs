using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using PlovCenter.Application.Common.Interfaces.Services;
using PlovCenter.Application.Common.Models;

namespace PlovCenter.Infrastructure.Services;

internal sealed class HttpCurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public CurrentUser GetCurrentUser()
    {
        var principal = httpContextAccessor.HttpContext?.User;

        if (principal?.Identity?.IsAuthenticated != true)
        {
            return new CurrentUser(null, null, false);
        }

        var rawUserId = principal.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? principal.FindFirstValue("sub");

        Guid? userId = Guid.TryParse(rawUserId, out var parsedUserId) ? parsedUserId : null;
        var username = principal.FindFirstValue(ClaimTypes.Name) ?? principal.FindFirstValue("unique_name");

        return new CurrentUser(userId, username, true);
    }
}
