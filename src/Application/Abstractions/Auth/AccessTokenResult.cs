namespace PlovCenter.Application.Abstractions.Auth;

public sealed record AccessTokenResult(string Token, DateTime ExpiresAtUtc);
