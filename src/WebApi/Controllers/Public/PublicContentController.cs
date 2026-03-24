using Microsoft.AspNetCore.Mvc;
using PlovCenter.Application.Common.Cqrs;
using PlovCenter.Application.Contract.Content;
using PlovCenter.Application.Features.Content;

namespace PlovCenter.WebApi.Controllers.Public;

[ApiController]
[Route("api/public/content")]
public sealed class PublicContentController(IRequestSender requestSender) : ControllerBase
{
    [HttpGet]
    public Task<PublicSiteContentResponse> GetContentAsync(CancellationToken cancellationToken)
    {
        return requestSender.SendAsync(new GetPublicSiteContentQuery(), cancellationToken);
    }
}
