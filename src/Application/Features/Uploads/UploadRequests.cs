using FluentValidation;
using PlovCenter.Application.Abstractions.Services;
using PlovCenter.Application.Common.Cqrs;
using PlovCenter.Application.Common.Exceptions;
using PlovCenter.Application.Contract.Uploads;

namespace PlovCenter.Application.Features.Uploads;

public sealed record UploadImageCommand(
    ImageUploadArea Area,
    string FileName,
    long Size,
    Stream Content) : IApplicationRequest<StoredFileResult>;

public sealed class UploadImageCommandValidator : AbstractValidator<UploadImageCommand>
{
    public UploadImageCommandValidator()
    {
        RuleFor(static command => command.Area).IsInEnum();
        RuleFor(static command => command.FileName).NotEmpty();
        RuleFor(static command => command.Size).GreaterThan(0);
    }
}

internal sealed class UploadImageCommandHandler(IFileStorageService fileStorageService)
    : IApplicationRequestHandler<UploadImageCommand, StoredFileResult>
{
    private const long MaxFileSizeInBytes = 5 * 1024 * 1024;

    private static readonly HashSet<string> AllowedExtensions =
    [
        ".jpg",
        ".jpeg",
        ".png"
    ];

    public async Task<StoredFileResult> HandleAsync(UploadImageCommand request, CancellationToken cancellationToken)
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

        return await fileStorageService.SaveImageAsync(
            new FileStorageRequest(request.Area, request.FileName, request.Size, request.Content),
            cancellationToken);
    }
}
