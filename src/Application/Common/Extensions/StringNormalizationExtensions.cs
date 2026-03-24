namespace PlovCenter.Application.Common.Extensions;

public static class StringNormalizationExtensions
{
    public static string NormalizeTrimmed(this string value)
    {
        return value.Trim();
    }

    public static string? NormalizeOptional(this string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    public static string NormalizeTrimmedLowerInvariant(this string value)
    {
        return value.Trim().ToLowerInvariant();
    }
}
