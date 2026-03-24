namespace PlovCenter.Application.Common.Models;

public sealed record CurrentUser(Guid? UserId, string? Username, bool IsAuthenticated);
