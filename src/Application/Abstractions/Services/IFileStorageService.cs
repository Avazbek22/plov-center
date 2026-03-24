namespace PlovCenter.Application.Abstractions.Services;

public interface IFileStorageService
{
    Task<StoredFileResult> SaveImageAsync(FileStorageRequest request, CancellationToken cancellationToken);
}
