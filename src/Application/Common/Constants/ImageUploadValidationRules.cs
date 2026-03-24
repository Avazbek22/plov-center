namespace PlovCenter.Application.Common.Constants;

public static class ImageUploadValidationRules
{
    public const long MaxFileSizeInBytes = 5 * 1024 * 1024;

    public static readonly HashSet<string> AllowedExtensions =
    [
        ".jpg",
        ".jpeg",
        ".png"
    ];
}
