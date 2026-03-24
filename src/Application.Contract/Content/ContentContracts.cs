namespace PlovCenter.Application.Contract.Content;

public sealed record AboutContentDto(string? Text, string? PhotoPath);

public sealed record ContactsContentDto(string? Address, string? Phone, string? Hours, string? MapEmbed);

public sealed record PublicSiteContentResponse(AboutContentDto About, ContactsContentDto Contacts);

public sealed record AdminSiteContentResponse(AboutContentDto About, ContactsContentDto Contacts);

public sealed record UpdateAboutContentRequest(string? Text, string? PhotoPath);

public sealed record UpdateContactsContentRequest(string? Address, string? Phone, string? Hours, string? MapEmbed);
