using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using PlovCenter.Application.Contract.Auth.Commands;
using PlovCenter.Application.Contract.Auth.Queries;
using PlovCenter.Application.Contract.Auth.Responses;
using PlovCenter.WebApi.Common;

namespace PlovCenter.WebApi.Controllers.Admin;

[ApiController]
[Route("api/admin/auth")]
public sealed class AdminAuthController(IMediator mediator) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    public Task<LoginResponse> LoginAsync([FromBody] LoginAdminCommand command, CancellationToken cancellationToken)
    {
        return mediator.Send(command, cancellationToken);
    }

    [Authorize(Policy = AuthorizationPolicies.AdminAccess)]
    [HttpGet("me")]
    public Task<CurrentAdminResponse> GetCurrentAdminAsync(CancellationToken cancellationToken)
    {
        return mediator.Send(new GetCurrentAdminQuery(), cancellationToken);
    }
}
