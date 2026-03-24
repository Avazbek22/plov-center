using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlovCenter.Application.Common.Cqrs;
using PlovCenter.Application.Contract.Categories;
using PlovCenter.Application.Features.Categories;

namespace PlovCenter.WebApi.Controllers.Admin;

[ApiController]
[Authorize]
[Route("api/admin/categories")]
public sealed class AdminCategoriesController(IRequestSender requestSender) : ControllerBase
{
    [HttpGet]
    public Task<IReadOnlyCollection<CategoryResponse>> GetCategories(CancellationToken cancellationToken)
    {
        return requestSender.SendAsync(new GetAdminCategoriesQuery(), cancellationToken);
    }

    [HttpGet("{categoryId:guid}")]
    public Task<CategoryResponse> GetCategory(Guid categoryId, CancellationToken cancellationToken)
    {
        return requestSender.SendAsync(new GetCategoryByIdQuery(categoryId), cancellationToken);
    }

    [HttpPost]
    public async Task<ActionResult<CategoryResponse>> CreateCategoryAsync(
        [FromBody] CreateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var response = await requestSender.SendAsync(
            new CreateCategoryCommand(request.Name, request.SortOrder, request.IsVisible),
            cancellationToken);

        return CreatedAtAction(nameof(GetCategory), new { categoryId = response.Id }, response);
    }

    [HttpPut("{categoryId:guid}")]
    public Task<CategoryResponse> UpdateCategoryAsync(
        Guid categoryId,
        [FromBody] UpdateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        return requestSender.SendAsync(
            new UpdateCategoryCommand(categoryId, request.Name, request.SortOrder, request.IsVisible),
            cancellationToken);
    }

    [HttpPatch("{categoryId:guid}/visibility")]
    public Task<CategoryResponse> SetVisibilityAsync(
        Guid categoryId,
        [FromBody] SetCategoryVisibilityRequest request,
        CancellationToken cancellationToken)
    {
        return requestSender.SendAsync(new SetCategoryVisibilityCommand(categoryId, request.IsVisible), cancellationToken);
    }

    [HttpPut("reorder")]
    public async Task<IActionResult> ReorderAsync([FromBody] ReorderCategoriesRequest request, CancellationToken cancellationToken)
    {
        await requestSender.SendAsync(new ReorderCategoriesCommand(request.Items), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{categoryId:guid}")]
    public async Task<IActionResult> DeleteCategoryAsync(Guid categoryId, CancellationToken cancellationToken)
    {
        await requestSender.SendAsync(new DeleteCategoryCommand(categoryId), cancellationToken);
        return NoContent();
    }
}
