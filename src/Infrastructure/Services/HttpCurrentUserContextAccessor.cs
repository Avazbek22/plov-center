using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using PlovCenter.Application.Abstractions.Services;

namespace PlovCenter.Infrastructure.Services;

internal sealed class HttpCurrentUserContextAccessor(IHttpContextAccessor httpContextAccessor) : ICurrentUserContextAccessor
{
    public CurrentUserContext GetCurrentUser()
    {
        var principal = httpContextAccessor.HttpContext?.User;

        if (principal?.Identity?.IsAuthenticated != true)
        {
            return new CurrentUserContext(null, null, false);
        }

        var rawUserId = principal.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? principal.FindFirstValue("sub");

        Guid? userId = Guid.TryParse(rawUserId, out var parsedUserId) ? parsedUserId : null;
        var username = principal.FindFirstValue(ClaimTypes.Name) ?? principal.FindFirstValue("unique_name");

        return new CurrentUserContext(userId, username, true);
    }
}
