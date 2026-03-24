using MediatR;
using PlovCenter.Application.Common.Exceptions;
using PlovCenter.Application.Common.Interfaces.Services;
using PlovCenter.Application.Contract.Uploads.Commands;
using PlovCenter.Application.Contract.Uploads.Responses;

namespace PlovCenter.Application.Features.Uploads.Commands;

public sealed class UploadImageCommandHandler(IFileStorageService fileStorageService)
    : IRequestHandler<UploadImageCommand, StoredUploadFileResponse>
{
    private const long MaxFileSizeInBytes = 5 * 1024 * 1024;

    private static readonly HashSet<string> AllowedExtensions =
    [
        ".jpg",
        ".jpeg",
        ".png"
    ];

    public async Task<StoredUploadFileResponse> Handle(UploadImageCommand request, CancellationToken cancellationToken)
    {
        if (request.Size > MaxFileSizeInBytes)
        {
            throw new RequestValidationException(new Dictionary<string, string[]>
            {
                [nameof(request.Size)] = ["Image size must be 5 MB or less."]
            });
        }

        var extension = Path.GetExtension(request.FileName);

        if (string.IsNullOrWhiteSpace(extension) || !AllowedExtensions.Contains(extension.ToLowerInvariant()))
        {
            throw new RequestValidationException(new Dictionary<string, string[]>
            {
                [nameof(request.FileName)] = ["Only JPG, JPEG, and PNG files are allowed."]
            });
        }

        var storedFile = await fileStorageService.SaveImageAsync(
            request.Area,
            request.FileName,
            request.Size,
            request.Content,
            cancellationToken);

        return new StoredUploadFileResponse(storedFile.RelativePath, storedFile.StoredFileName, storedFile.Size);
    }
}
