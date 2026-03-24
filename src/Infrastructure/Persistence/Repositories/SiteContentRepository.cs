using Microsoft.EntityFrameworkCore;
using PlovCenter.Application.Abstractions.Persistence;
using PlovCenter.Domain.Entities;

namespace PlovCenter.Infrastructure.Persistence.Repositories;

internal sealed class SiteContentRepository(PlovCenterDbContext dbContext) : ISiteContentRepository
{
    public async Task<IDictionary<string, SiteContentEntry>> GetByKeysAsync(
        IReadOnlyCollection<string> keys,
        CancellationToken cancellationToken)
    {
        return await dbContext.SiteContentEntries
            .Where(entry => keys.Contains(entry.Key))
            .ToDictionaryAsync(entry => entry.Key, cancellationToken);
    }

    public void AddRange(IEnumerable<SiteContentEntry> entries)
    {
        dbContext.SiteContentEntries.AddRange(entries);
    }
}
