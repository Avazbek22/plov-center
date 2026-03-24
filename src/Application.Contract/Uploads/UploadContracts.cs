namespace PlovCenter.Application.Contract.Uploads;

public enum ImageUploadArea
{
    Dish = 1,
    About = 2
}

public sealed record UploadImageResponse(
    string RelativePath,
    string Url,
    string FileName,
    long Size);
