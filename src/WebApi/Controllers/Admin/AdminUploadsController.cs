using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using PlovCenter.Application.Contract.Uploads.Commands;
using PlovCenter.Application.Contract.Uploads.Responses;
using PlovCenter.WebApi.Common;

namespace PlovCenter.WebApi.Controllers.Admin;

[ApiController]
[Authorize]
[Route("api/admin/uploads")]
public sealed class AdminUploadsController(IMediator mediator) : ControllerBase
{
    [HttpPost("image")]
    [Consumes("multipart/form-data")]
    public async Task<UploadImageResponse> UploadImageAsync(
        [FromForm] UploadImageForm form,
        CancellationToken cancellationToken)
    {
        await using var stream = form.File.OpenReadStream();

        var result = await mediator.Send(
            new UploadImageCommand
            {
                Area = form.Area,
                FileName = form.File.FileName,
                Size = form.File.Length,
                Content = stream
            },
            cancellationToken);

        return new UploadImageResponse(
            result.RelativePath,
            BuildFileUrl(result.RelativePath),
            result.StoredFileName,
            result.Size);
    }

    private string BuildFileUrl(string relativePath)
    {
        return $"{Request.Scheme}://{Request.Host}{Request.PathBase}/uploads/{relativePath}";
    }
}
