using PlovCenter.Application.Abstractions.Services;
using PlovCenter.Application.Contract.Uploads;
using Microsoft.Extensions.Options;
using PlovCenter.Infrastructure.Configuration;

namespace PlovCenter.Infrastructure.Services;

internal sealed class LocalFileStorageService(IOptions<FileStorageOptions> fileStorageOptions) : IFileStorageService
{
    public async Task<StoredFileResult> SaveImageAsync(FileStorageRequest request, CancellationToken cancellationToken)
    {
        var rootPath = fileStorageOptions.Value.RootPath;
        var extension = Path.GetExtension(request.FileName).ToLowerInvariant();
        var areaFolder = ResolveAreaFolder(request.Area);
        var datePath = Path.Combine(DateTime.UtcNow.ToString("yyyy"), DateTime.UtcNow.ToString("MM"));
        var targetDirectory = Path.Combine(rootPath, areaFolder, datePath);

        Directory.CreateDirectory(targetDirectory);

        var storedFileName = $"{Guid.NewGuid():N}{extension}";
        var absolutePath = Path.Combine(targetDirectory, storedFileName);

        await using var fileStream = new FileStream(
            absolutePath,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None,
            81920,
            useAsync: true);

        await request.Content.CopyToAsync(fileStream, cancellationToken);

        var relativePath = Path.Combine(areaFolder, datePath, storedFileName)
            .Replace('\\', '/');

        return new StoredFileResult(relativePath, storedFileName, request.Size);
    }

    private static string ResolveAreaFolder(ImageUploadArea area)
    {
        return area switch
        {
            ImageUploadArea.Dish => "dishes",
            ImageUploadArea.About => "about",
            _ => throw new ArgumentOutOfRangeException(nameof(area), area, "Unsupported upload area.")
        };
    }
}
