using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using PlovCenter.Application.Contract.Dishes.Commands;
using PlovCenter.Application.Contract.Dishes.Queries;
using PlovCenter.Application.Contract.Dishes.Responses;

namespace PlovCenter.WebApi.Controllers.Admin;

[ApiController]
[Authorize]
[Route("api/admin/dishes")]
public sealed class AdminDishesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public Task<IReadOnlyCollection<DishResponse>> GetDishes([FromQuery] Guid? categoryId, CancellationToken cancellationToken)
    {
        return mediator.Send(new GetAdminDishesQuery { CategoryId = categoryId }, cancellationToken);
    }

    [HttpGet("{dishId:guid}")]
    public Task<DishResponse> GetDish(Guid dishId, CancellationToken cancellationToken)
    {
        return mediator.Send(new GetDishByIdQuery(dishId), cancellationToken);
    }

    [HttpPost]
    public async Task<ActionResult<DishResponse>> CreateDishAsync(
        [FromBody] CreateDishCommand command,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetDish), new { dishId = response.Id }, response);
    }

    [HttpPut("{dishId:guid}")]
    public Task<DishResponse> UpdateDishAsync(
        Guid dishId,
        [FromBody] UpdateDishCommand command,
        CancellationToken cancellationToken)
    {
        command.DishId = dishId;
        return mediator.Send(command, cancellationToken);
    }

    [HttpPatch("{dishId:guid}/visibility")]
    public Task<DishResponse> SetVisibilityAsync(
        Guid dishId,
        [FromBody] SetDishVisibilityCommand command,
        CancellationToken cancellationToken)
    {
        command.DishId = dishId;
        return mediator.Send(command, cancellationToken);
    }

    [HttpDelete("{dishId:guid}")]
    public async Task<IActionResult> DeleteDishAsync(Guid dishId, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteDishCommand(dishId), cancellationToken);
        return NoContent();
    }
}
