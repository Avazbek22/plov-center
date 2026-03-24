using PlovCenter.Application.Common.Models;
using PlovCenter.Application.Contract.Uploads;

namespace PlovCenter.Application.Common.Interfaces.Services;

public interface IFileStorageService
{
    Task<StoredFileResult> SaveImageAsync(
        ImageUploadArea area,
        string fileName,
        long size,
        Stream content,
        CancellationToken cancellationToken);
}
