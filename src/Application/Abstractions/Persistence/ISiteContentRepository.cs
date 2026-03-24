using PlovCenter.Domain.Entities;

namespace PlovCenter.Application.Abstractions.Persistence;

public interface ISiteContentRepository
{
    Task<IDictionary<string, SiteContentEntry>> GetByKeysAsync(
        IReadOnlyCollection<string> keys,
        CancellationToken cancellationToken);

    void AddRange(IEnumerable<SiteContentEntry> entries);
}
