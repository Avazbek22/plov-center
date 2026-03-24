using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlovCenter.Application.Common.Cqrs;
using PlovCenter.Application.Contract.Uploads;
using PlovCenter.Application.Features.Uploads;
using PlovCenter.WebApi.Common;

namespace PlovCenter.WebApi.Controllers.Admin;

[ApiController]
[Authorize]
[Route("api/admin/uploads")]
public sealed class AdminUploadsController(IRequestSender requestSender) : ControllerBase
{
    [HttpPost("image")]
    [Consumes("multipart/form-data")]
    public async Task<UploadImageResponse> UploadImageAsync(
        [FromForm] UploadImageForm form,
        CancellationToken cancellationToken)
    {
        await using var stream = form.File.OpenReadStream();

        var result = await requestSender.SendAsync(
            new UploadImageCommand(form.Area, form.File.FileName, form.File.Length, stream),
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
