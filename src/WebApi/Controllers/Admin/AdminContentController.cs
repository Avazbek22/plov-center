using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlovCenter.Application.Common.Cqrs;
using PlovCenter.Application.Contract.Content;
using PlovCenter.Application.Features.Content;

namespace PlovCenter.WebApi.Controllers.Admin;

[ApiController]
[Authorize]
[Route("api/admin/content")]
public sealed class AdminContentController(IRequestSender requestSender) : ControllerBase
{
    [HttpGet]
    public Task<AdminSiteContentResponse> GetContentAsync(CancellationToken cancellationToken)
    {
        return requestSender.SendAsync(new GetAdminSiteContentQuery(), cancellationToken);
    }

    [HttpPut("about")]
    public Task<AdminSiteContentResponse> UpdateAboutAsync(
        [FromBody] UpdateAboutContentRequest request,
        CancellationToken cancellationToken)
    {
        return requestSender.SendAsync(new UpdateAboutContentCommand(request.Text, request.PhotoPath), cancellationToken);
    }

    [HttpPut("contacts")]
    public Task<AdminSiteContentResponse> UpdateContactsAsync(
        [FromBody] UpdateContactsContentRequest request,
        CancellationToken cancellationToken)
    {
        return requestSender.SendAsync(
            new UpdateContactsContentCommand(request.Address, request.Phone, request.Hours, request.MapEmbed),
            cancellationToken);
    }
}
