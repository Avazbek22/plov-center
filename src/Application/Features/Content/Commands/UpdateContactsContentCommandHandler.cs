using MediatR;
using Microsoft.EntityFrameworkCore;
using PlovCenter.Application.Common.Constants;
using PlovCenter.Application.Common.Interfaces.Contexts;
using PlovCenter.Application.Common.Interfaces.Services;
using PlovCenter.Application.Contract.Content.Commands;
using PlovCenter.Application.Contract.Content.Responses;
using PlovCenter.Application.Features.Content.Mappings;
using PlovCenter.Domain.Entities;

namespace PlovCenter.Application.Features.Content.Commands;

public sealed class UpdateContactsContentCommandHandler(
    IApplicationDbContext applicationDbContext,
    IDateTimeService dateTimeService) : IRequestHandler<UpdateContactsContentCommand, AdminSiteContentResponse>
{
    public async Task<AdminSiteContentResponse> Handle(UpdateContactsContentCommand request, CancellationToken cancellationToken)
    {
        var entries = await LoadEntriesAsync(cancellationToken);
        var utcNow = dateTimeService.UtcNow;

        Upsert(entries, SiteContentKeys.ContactsAddress, NormalizeOptionalValue(request.Address), utcNow);
        Upsert(entries, SiteContentKeys.ContactsPhone, NormalizeOptionalValue(request.Phone), utcNow);
        Upsert(entries, SiteContentKeys.ContactsHours, NormalizeOptionalValue(request.Hours), utcNow);
        Upsert(entries, SiteContentKeys.ContactsMapEmbed, NormalizeOptionalValue(request.MapEmbed), utcNow);

        await applicationDbContext.SaveChangesAsync(cancellationToken);

        return entries.ToAdminResponse();
    }

    private async Task<Dictionary<string, SiteContentEntry>> LoadEntriesAsync(CancellationToken cancellationToken)
    {
        return await applicationDbContext.SiteContentEntries
            .Where(entry => SiteContentKeys.All.Contains(entry.Key))
            .ToDictionaryAsync(entry => entry.Key, cancellationToken);
    }

    private void Upsert(IDictionary<string, SiteContentEntry> entries, string key, string? value, DateTime utcNow)
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

    private static string? NormalizeOptionalValue(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
