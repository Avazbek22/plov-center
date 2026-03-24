using System.Buffers;
using PlovCenter.Application.Contract.Uploads;
using PlovCenter.Application.Common.Constants;
using PlovCenter.Application.Common.Exceptions;
using PlovCenter.Application.Common.Interfaces.Services;
using PlovCenter.Application.Common.Models;
using Microsoft.Extensions.Options;
using PlovCenter.Infrastructure.Configuration;

namespace PlovCenter.Infrastructure.Services;

internal sealed class LocalFileStorageService(
    IOptions<FileStorageOptions> fileStorageOptions,
    IDateTimeService dateTimeService) : IFileStorageService
{
    public async Task<StoredFileResult> SaveImageAsync(
        ImageUploadArea area,
        string fileName,
        long size,
        Stream content,
        CancellationToken cancellationToken)
    {
        var rootPath = fileStorageOptions.Value.RootPath;
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        var areaFolder = ResolveAreaFolder(area);
        var utcNow = dateTimeService.UtcNow;
        var datePath = Path.Combine(utcNow.ToString("yyyy"), utcNow.ToString("MM"));
        var targetDirectory = Path.Combine(rootPath, areaFolder, datePath);
        var storedFileName = $"{Guid.NewGuid():N}{extension}";
        var absolutePath = Path.Combine(targetDirectory, storedFileName);

        var validationBuffer = ArrayPool<byte>.Shared.Rent(81920);
        var headerBuffer = ArrayPool<byte>.Shared.Rent(8);

        try
        {
            var headerBytesRead = await ReadHeaderAsync(content, headerBuffer, cancellationToken);

            if (headerBytesRead == 0 || !ImageFileSignatureValidator.Matches(extension, headerBuffer.AsSpan(0, headerBytesRead)))
            {
                throw CreateValidationException("FileName", "The uploaded file content does not match a supported JPG, JPEG, or PNG image.");
            }

            long totalBytesRead = headerBytesRead;

            if (totalBytesRead > ImageUploadValidationRules.MaxFileSizeInBytes)
            {
                throw CreateValidationException("Size", "Image size must be 5 MB or less.");
            }

            Directory.CreateDirectory(targetDirectory);

            await using var fileStream = new FileStream(
                absolutePath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None,
                81920,
                useAsync: true);

            await fileStream.WriteAsync(headerBuffer.AsMemory(0, headerBytesRead), cancellationToken);

            while (true)
            {
                var bytesRead = await content.ReadAsync(validationBuffer.AsMemory(0, validationBuffer.Length), cancellationToken);

                if (bytesRead == 0)
                {
                    break;
                }

                totalBytesRead += bytesRead;

                if (totalBytesRead > ImageUploadValidationRules.MaxFileSizeInBytes)
                {
                    throw CreateValidationException("Size", "Image size must be 5 MB or less.");
                }

                await fileStream.WriteAsync(validationBuffer.AsMemory(0, bytesRead), cancellationToken);
            }

            var relativePath = Path.Combine(areaFolder, datePath, storedFileName)
                .Replace('\\', '/');

            return new StoredFileResult(relativePath, storedFileName, totalBytesRead);
        }
        catch
        {
            if (File.Exists(absolutePath))
            {
                File.Delete(absolutePath);
            }

            throw;
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(validationBuffer);
            ArrayPool<byte>.Shared.Return(headerBuffer);
        }
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

    private static async Task<int> ReadHeaderAsync(Stream content, byte[] headerBuffer, CancellationToken cancellationToken)
    {
        var totalBytesRead = 0;

        while (totalBytesRead < headerBuffer.Length)
        {
            var bytesRead = await content.ReadAsync(
                headerBuffer.AsMemory(totalBytesRead, headerBuffer.Length - totalBytesRead),
                cancellationToken);

            if (bytesRead == 0)
            {
                break;
            }

            totalBytesRead += bytesRead;
        }

        return totalBytesRead;
    }

    private static RequestValidationException CreateValidationException(string propertyName, string message)
    {
        return new RequestValidationException(new Dictionary<string, string[]>
        {
            [propertyName] = [message]
        });
    }
}
