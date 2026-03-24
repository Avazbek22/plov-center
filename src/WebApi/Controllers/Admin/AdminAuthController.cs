using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlovCenter.Application.Common.Cqrs;
using PlovCenter.Application.Contract.Auth;
using PlovCenter.Application.Features.Auth;

namespace PlovCenter.WebApi.Controllers.Admin;

[ApiController]
[Route("api/admin/auth")]
public sealed class AdminAuthController(IRequestSender requestSender) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    public Task<LoginResponse> LoginAsync([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        return requestSender.SendAsync(new LoginAdminCommand(request.Username, request.Password), cancellationToken);
    }

    [Authorize]
    [HttpGet("me")]
    public Task<CurrentAdminResponse> GetCurrentAdminAsync(CancellationToken cancellationToken)
    {
        return requestSender.SendAsync(new GetCurrentAdminQuery(), cancellationToken);
    }
}
