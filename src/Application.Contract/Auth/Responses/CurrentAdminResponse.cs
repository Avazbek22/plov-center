namespace PlovCenter.Application.Contract.Auth.Responses;

public sealed record CurrentAdminResponse(Guid Id, string Username, bool IsActive);
