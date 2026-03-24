using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using PlovCenter.Application.Contract.Content.Commands;
using PlovCenter.Application.Contract.Content.Queries;
using PlovCenter.Application.Contract.Content.Responses;

namespace PlovCenter.WebApi.Controllers.Admin;

[ApiController]
[Authorize]
[Route("api/admin/content")]
public sealed class AdminContentController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public Task<AdminSiteContentResponse> GetContentAsync(CancellationToken cancellationToken)
    {
        return mediator.Send(new GetAdminSiteContentQuery(), cancellationToken);
    }

    [HttpPut("about")]
    public Task<AdminSiteContentResponse> UpdateAboutAsync(
        [FromBody] UpdateAboutContentCommand command,
        CancellationToken cancellationToken)
    {
        return mediator.Send(command, cancellationToken);
    }

    [HttpPut("contacts")]
    public Task<AdminSiteContentResponse> UpdateContactsAsync(
        [FromBody] UpdateContactsContentCommand command,
        CancellationToken cancellationToken)
    {
        return mediator.Send(command, cancellationToken);
    }
}
