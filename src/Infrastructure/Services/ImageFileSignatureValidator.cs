namespace PlovCenter.Infrastructure.Services;

internal static class ImageFileSignatureValidator
{
    private static readonly byte[] PngSignature = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];
    private static readonly byte[] JpegSignaturePrefix = [0xFF, 0xD8, 0xFF];

    public static bool Matches(string extension, ReadOnlySpan<byte> headerBytes)
    {
        return extension.ToLowerInvariant() switch
        {
            ".png" => headerBytes.StartsWith(PngSignature),
            ".jpg" or ".jpeg" => headerBytes.StartsWith(JpegSignaturePrefix),
            _ => false
        };
    }
}
