using Microsoft.AspNetCore.Http;
using PlovCenter.Application.Contract.Uploads;

namespace PlovCenter.WebApi.Common;

public sealed class UploadImageForm
{
    public ImageUploadArea Area { get; set; }

    public IFormFile File { get; set; } = default!;
}
