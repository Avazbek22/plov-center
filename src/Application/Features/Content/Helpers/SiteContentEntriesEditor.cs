using Microsoft.EntityFrameworkCore;
using PlovCenter.Application.Common.Constants;
using PlovCenter.Application.Common.Interfaces.Contexts;
using PlovCenter.Domain.Entities;

namespace PlovCenter.Application.Features.Content.Helpers;

internal static class SiteContentEntriesEditor
{
    public static Task<Dictionary<string, SiteContentEntry>> LoadAsync(
        IApplicationDbContext applicationDbContext,
        CancellationToken cancellationToken)
    {
        return applicationDbContext.SiteContentEntries
            .Where(entry => SiteContentKeys.All.Contains(entry.Key))
            .ToDictionaryAsync(entry => entry.Key, cancellationToken);
    }

    public static void Upsert(
        IApplicationDbContext applicationDbContext,
        IDictionary<string, SiteContentEntry> entries,
        string key,
        string? value,
        DateTime utcNow)
    {
        if (entries.TryGetValue(key, out var entry))
        {
            entry.Value = value;
            entry.UpdatedUtc = utcNow;
            return;
        }

        var newEntry = new SiteContentEntry
        {
            Id = Guid.NewGuid(),
            Key = key,
            Value = value,
            CreatedUtc = utcNow,
            UpdatedUtc = utcNow
        };

        applicationDbContext.SiteContentEntries.Add(newEntry);
        entries[key] = newEntry;
    }
}
