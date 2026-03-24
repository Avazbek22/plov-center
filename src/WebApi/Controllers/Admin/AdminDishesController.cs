using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlovCenter.Application.Common.Cqrs;
using PlovCenter.Application.Contract.Dishes;
using PlovCenter.Application.Features.Dishes;

namespace PlovCenter.WebApi.Controllers.Admin;

[ApiController]
[Authorize]
[Route("api/admin/dishes")]
public sealed class AdminDishesController(IRequestSender requestSender) : ControllerBase
{
    [HttpGet]
    public Task<IReadOnlyCollection<DishResponse>> GetDishes([FromQuery] Guid? categoryId, CancellationToken cancellationToken)
    {
        return requestSender.SendAsync(new GetAdminDishesQuery(categoryId), cancellationToken);
    }

    [HttpGet("{dishId:guid}")]
    public Task<DishResponse> GetDish(Guid dishId, CancellationToken cancellationToken)
    {
        return requestSender.SendAsync(new GetDishByIdQuery(dishId), cancellationToken);
    }

    [HttpPost]
    public async Task<ActionResult<DishResponse>> CreateDishAsync(
        [FromBody] CreateDishRequest request,
        CancellationToken cancellationToken)
    {
        var response = await requestSender.SendAsync(
            new CreateDishCommand(
                request.CategoryId,
                request.Name,
                request.Description,
                request.Price,
                request.PhotoPath,
                request.SortOrder,
                request.IsVisible),
            cancellationToken);

        return CreatedAtAction(nameof(GetDish), new { dishId = response.Id }, response);
    }

    [HttpPut("{dishId:guid}")]
    public Task<DishResponse> UpdateDishAsync(
        Guid dishId,
        [FromBody] UpdateDishRequest request,
        CancellationToken cancellationToken)
    {
        return requestSender.SendAsync(
            new UpdateDishCommand(
                dishId,
                request.CategoryId,
                request.Name,
                request.Description,
                request.Price,
                request.PhotoPath,
                request.SortOrder,
                request.IsVisible),
            cancellationToken);
    }

    [HttpPatch("{dishId:guid}/visibility")]
    public Task<DishResponse> SetVisibilityAsync(
        Guid dishId,
        [FromBody] SetDishVisibilityRequest request,
        CancellationToken cancellationToken)
    {
        return requestSender.SendAsync(new SetDishVisibilityCommand(dishId, request.IsVisible), cancellationToken);
    }

    [HttpDelete("{dishId:guid}")]
    public async Task<IActionResult> DeleteDishAsync(Guid dishId, CancellationToken cancellationToken)
    {
        await requestSender.SendAsync(new DeleteDishCommand(dishId), cancellationToken);
        return NoContent();
    }
}
