using PlovCenter.Application.Contract.Uploads;

namespace PlovCenter.Application.Abstractions.Services;

public sealed record FileStorageRequest(
    ImageUploadArea Area,
    string FileName,
    long Size,
    Stream Content);

public sealed record StoredFileResult(
    string RelativePath,
    string StoredFileName,
    long Size);
