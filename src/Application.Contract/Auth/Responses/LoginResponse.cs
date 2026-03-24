namespace PlovCenter.Application.Contract.Auth.Responses;

public sealed record LoginResponse(string Token, DateTime ExpiresAtUtc, CurrentAdminResponse Admin);
