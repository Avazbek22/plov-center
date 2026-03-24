using MediatR;
using PlovCenter.Application.Contract.Uploads.Responses;

namespace PlovCenter.Application.Contract.Uploads.Commands;

public sealed class UploadImageCommand : IRequest<StoredUploadFileResponse>
{
    public ImageUploadArea Area { get; set; }

    public string FileName { get; set; } = string.Empty;

    public long Size { get; set; }

    public Stream Content { get; set; } = Stream.Null;
}
