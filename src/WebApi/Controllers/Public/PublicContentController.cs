using Microsoft.AspNetCore.Mvc;
using MediatR;
using PlovCenter.Application.Contract.Content.Queries;
using PlovCenter.Application.Contract.Content.Responses;

namespace PlovCenter.WebApi.Controllers.Public;

[ApiController]
[Route("api/public/content")]
public sealed class PublicContentController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public Task<PublicSiteContentResponse> GetContentAsync(CancellationToken cancellationToken)
    {
        return mediator.Send(new GetPublicSiteContentQuery(), cancellationToken);
    }
}
