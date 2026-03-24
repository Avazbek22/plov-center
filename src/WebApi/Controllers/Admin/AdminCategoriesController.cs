using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using PlovCenter.Application.Contract.Categories.Commands;
using PlovCenter.Application.Contract.Categories.Queries;
using PlovCenter.Application.Contract.Categories.Responses;

namespace PlovCenter.WebApi.Controllers.Admin;

[ApiController]
[Authorize]
[Route("api/admin/categories")]
public sealed class AdminCategoriesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public Task<IReadOnlyCollection<CategoryResponse>> GetCategories(CancellationToken cancellationToken)
    {
        return mediator.Send(new GetAdminCategoriesQuery(), cancellationToken);
    }

    [HttpGet("{categoryId:guid}")]
    public Task<CategoryResponse> GetCategory(Guid categoryId, CancellationToken cancellationToken)
    {
        return mediator.Send(new GetCategoryByIdQuery(categoryId), cancellationToken);
    }

    [HttpPost]
    public async Task<ActionResult<CategoryResponse>> CreateCategoryAsync(
        [FromBody] CreateCategoryCommand command,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetCategory), new { categoryId = response.Id }, response);
    }

    [HttpPut("{categoryId:guid}")]
    public Task<CategoryResponse> UpdateCategoryAsync(
        Guid categoryId,
        [FromBody] UpdateCategoryCommand command,
        CancellationToken cancellationToken)
    {
        command.CategoryId = categoryId;
        return mediator.Send(command, cancellationToken);
    }

    [HttpPatch("{categoryId:guid}/visibility")]
    public Task<CategoryResponse> SetVisibilityAsync(
        Guid categoryId,
        [FromBody] SetCategoryVisibilityCommand command,
        CancellationToken cancellationToken)
    {
        command.CategoryId = categoryId;
        return mediator.Send(command, cancellationToken);
    }

    [HttpPut("reorder")]
    public async Task<IActionResult> ReorderAsync([FromBody] ReorderCategoriesCommand command, CancellationToken cancellationToken)
    {
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{categoryId:guid}")]
    public async Task<IActionResult> DeleteCategoryAsync(Guid categoryId, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteCategoryCommand(categoryId), cancellationToken);
        return NoContent();
    }
}
