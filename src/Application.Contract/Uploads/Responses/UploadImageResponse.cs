namespace PlovCenter.Application.Contract.Uploads.Responses;

public sealed record UploadImageResponse(
    string RelativePath,
    string Url,
    string FileName,
    long Size);
