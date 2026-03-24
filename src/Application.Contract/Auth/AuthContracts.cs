namespace PlovCenter.Application.Contract.Auth;

public sealed record LoginRequest(string Username, string Password);

public sealed record CurrentAdminResponse(Guid Id, string Username, bool IsActive);

public sealed record LoginResponse(string Token, DateTime ExpiresAtUtc, CurrentAdminResponse Admin);
