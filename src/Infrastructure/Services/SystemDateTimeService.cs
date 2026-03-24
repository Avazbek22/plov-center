using PlovCenter.Application.Common.Interfaces.Services;

namespace PlovCenter.Infrastructure.Services;

internal sealed class SystemDateTimeService : IDateTimeService
{
    public DateTime UtcNow => DateTime.UtcNow;
}
