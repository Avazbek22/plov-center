using Microsoft.AspNetCore.Mvc;
using MediatR;
using PlovCenter.Application.Contract.Menu.Queries;
using PlovCenter.Application.Contract.Menu.Responses;

namespace PlovCenter.WebApi.Controllers.Public;

[ApiController]
[Route("api/public/menu")]
public sealed class PublicMenuController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public Task<PublicMenuResponse> GetMenuAsync(CancellationToken cancellationToken)
    {
        return mediator.Send(new GetPublicMenuQuery(), cancellationToken);
    }
}
