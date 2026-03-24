namespace PlovCenter.Application.Abstractions.Services;

public sealed record CurrentUserContext(Guid? UserId, string? Username, bool IsAuthenticated);
