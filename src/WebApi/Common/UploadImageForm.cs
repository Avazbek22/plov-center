using System.ComponentModel.DataAnnotations;
using PlovCenter.Application.Contract.Uploads;

namespace PlovCenter.WebApi.Common;

public sealed class UploadImageForm
{
    public ImageUploadArea Area { get; set; }

    [Required]
    public IFormFile File { get; set; } = default!;
}
