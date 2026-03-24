namespace PlovCenter.Application.Contract.Content.Responses;

public sealed record ContactsContentResponse(string? Address, string? Phone, string? Hours, string? MapEmbed);
