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

public sealed class UpdateAboutContentCommandHandler(
    IApplicationDbContext applicationDbContext,
    IDateTimeService dateTimeService) : IRequestHandler<UpdateAboutContentCommand, AdminSiteContentResponse>
{
    public async Task<AdminSiteContentResponse> Handle(UpdateAboutContentCommand request, CancellationToken cancellationToken)
    {
        var entries = await LoadEntriesAsync(cancellationToken);
        var utcNow = dateTimeService.UtcNow;

        Upsert(entries, SiteContentKeys.AboutText, NormalizeOptionalValue(request.Text), utcNow);
        Upsert(entries, SiteContentKeys.AboutPhoto, NormalizeOptionalValue(request.PhotoPath), utcNow);

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
