namespace PlovCenter.Application.Contract.Uploads.Responses;

public sealed record StoredUploadFileResponse(
    string RelativePath,
    string StoredFileName,
    long Size);
