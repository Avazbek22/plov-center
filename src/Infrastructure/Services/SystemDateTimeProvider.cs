using PlovCenter.Application.Abstractions.Services;

namespace PlovCenter.Infrastructure.Services;

internal sealed class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
