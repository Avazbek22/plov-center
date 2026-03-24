using PlovCenter.Application.Common.Constants;
using PlovCenter.Application.Contract.Content.Responses;
using PlovCenter.Domain.Entities;

namespace PlovCenter.Application.Features.Content.Mappings;

internal static class SiteContentResponseMappings
{
    public static PublicSiteContentResponse ToPublicResponse(this IReadOnlyDictionary<string, SiteContentEntry> entries)
    {
        return new PublicSiteContentResponse(entries.ToAboutResponse(), entries.ToContactsResponse());
    }

    public static AdminSiteContentResponse ToAdminResponse(this IReadOnlyDictionary<string, SiteContentEntry> entries)
    {
        return new AdminSiteContentResponse(entries.ToAboutResponse(), entries.ToContactsResponse());
    }

    public static AboutContentResponse ToAboutResponse(this IReadOnlyDictionary<string, SiteContentEntry> entries)
    {
        return new AboutContentResponse(
            GetValue(entries, SiteContentKeys.AboutText),
            GetValue(entries, SiteContentKeys.AboutPhoto));
    }

    public static ContactsContentResponse ToContactsResponse(this IReadOnlyDictionary<string, SiteContentEntry> entries)
    {
        return new ContactsContentResponse(
            GetValue(entries, SiteContentKeys.ContactsAddress),
            GetValue(entries, SiteContentKeys.ContactsPhone),
            GetValue(entries, SiteContentKeys.ContactsHours),
            GetValue(entries, SiteContentKeys.ContactsMapEmbed));
    }

    private static string? GetValue(IReadOnlyDictionary<string, SiteContentEntry> entries, string key)
    {
        return entries.TryGetValue(key, out var entry) ? entry.Value : null;
    }
}
