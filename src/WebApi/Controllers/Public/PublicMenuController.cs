using Microsoft.AspNetCore.Mvc;
using PlovCenter.Application.Common.Cqrs;
using PlovCenter.Application.Contract.Menu;
using PlovCenter.Application.Features.Menu;

namespace PlovCenter.WebApi.Controllers.Public;

[ApiController]
[Route("api/public/menu")]
public sealed class PublicMenuController(IRequestSender requestSender) : ControllerBase
{
    [HttpGet]
    public Task<PublicMenuResponse> GetMenuAsync(CancellationToken cancellationToken)
    {
        return requestSender.SendAsync(new GetPublicMenuQuery(), cancellationToken);
    }
}
