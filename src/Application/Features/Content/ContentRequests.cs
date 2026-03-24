using FluentValidation;
using PlovCenter.Application.Abstractions.Persistence;
using PlovCenter.Application.Abstractions.Services;
using PlovCenter.Application.Common.Cqrs;
using PlovCenter.Application.Common.Validation;
using PlovCenter.Application.Contract.Content;
using PlovCenter.Domain.Entities;

namespace PlovCenter.Application.Features.Content;

public sealed record GetPublicSiteContentQuery() : IApplicationRequest<PublicSiteContentResponse>;

public sealed record GetAdminSiteContentQuery() : IApplicationRequest<AdminSiteContentResponse>;

public sealed record UpdateAboutContentCommand(string? Text, string? PhotoPath) : IApplicationRequest<AdminSiteContentResponse>;

public sealed record UpdateContactsContentCommand(string? Address, string? Phone, string? Hours, string? MapEmbed)
    : IApplicationRequest<AdminSiteContentResponse>;

public sealed class UpdateAboutContentCommandValidator : AbstractValidator<UpdateAboutContentCommand>
{
    public UpdateAboutContentCommandValidator()
    {
        RuleFor(static command => command.Text).MaximumLength(ValidationRules.ContentValueMaxLength);
        RuleFor(static command => command.PhotoPath).MaximumLength(512);
    }
}

public sealed class UpdateContactsContentCommandValidator : AbstractValidator<UpdateContactsContentCommand>
{
    public UpdateContactsContentCommandValidator()
    {
        RuleFor(static command => command.Address).MaximumLength(ValidationRules.ContentValueMaxLength);
        RuleFor(static command => command.Phone).MaximumLength(250);
        RuleFor(static command => command.Hours).MaximumLength(500);
        RuleFor(static command => command.MapEmbed).MaximumLength(ValidationRules.ContentValueMaxLength);
    }
}

internal sealed class GetPublicSiteContentQueryHandler(ISiteContentRepository siteContentRepository)
    : IApplicationRequestHandler<GetPublicSiteContentQuery, PublicSiteContentResponse>
{
    public async Task<PublicSiteContentResponse> HandleAsync(
        GetPublicSiteContentQuery request,
        CancellationToken cancellationToken)
    {
        var entries = await siteContentRepository.GetByKeysAsync(SiteContentKeys.All, cancellationToken);
        return new PublicSiteContentResponse(ContentMappings.MapAbout(entries), ContentMappings.MapContacts(entries));
    }
}

internal sealed class GetAdminSiteContentQueryHandler(ISiteContentRepository siteContentRepository)
    : IApplicationRequestHandler<GetAdminSiteContentQuery, AdminSiteContentResponse>
{
    public async Task<AdminSiteContentResponse> HandleAsync(
        GetAdminSiteContentQuery request,
        CancellationToken cancellationToken)
    {
        var entries = await siteContentRepository.GetByKeysAsync(SiteContentKeys.All, cancellationToken);
        return new AdminSiteContentResponse(ContentMappings.MapAbout(entries), ContentMappings.MapContacts(entries));
    }
}

internal sealed class UpdateAboutContentCommandHandler(
    ISiteContentRepository siteContentRepository,
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider)
    : IApplicationRequestHandler<UpdateAboutContentCommand, AdminSiteContentResponse>
{
    public async Task<AdminSiteContentResponse> HandleAsync(
        UpdateAboutContentCommand request,
        CancellationToken cancellationToken)
    {
        var entries = await siteContentRepository.GetByKeysAsync(SiteContentKeys.All, cancellationToken);

        ContentMappings.UpsertEntry(
            entries,
            SiteContentKeys.AboutText,
            request.Text,
            dateTimeProvider.UtcNow,
            siteContentRepository);

        ContentMappings.UpsertEntry(
            entries,
            SiteContentKeys.AboutPhoto,
            request.PhotoPath,
            dateTimeProvider.UtcNow,
            siteContentRepository);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new AdminSiteContentResponse(ContentMappings.MapAbout(entries), ContentMappings.MapContacts(entries));
    }
}

internal sealed class UpdateContactsContentCommandHandler(
    ISiteContentRepository siteContentRepository,
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider)
    : IApplicationRequestHandler<UpdateContactsContentCommand, AdminSiteContentResponse>
{
    public async Task<AdminSiteContentResponse> HandleAsync(
        UpdateContactsContentCommand request,
        CancellationToken cancellationToken)
    {
        var entries = await siteContentRepository.GetByKeysAsync(SiteContentKeys.All, cancellationToken);

        ContentMappings.UpsertEntry(
            entries,
            SiteContentKeys.ContactsAddress,
            request.Address,
            dateTimeProvider.UtcNow,
            siteContentRepository);

        ContentMappings.UpsertEntry(
            entries,
            SiteContentKeys.ContactsPhone,
            request.Phone,
            dateTimeProvider.UtcNow,
            siteContentRepository);

        ContentMappings.UpsertEntry(
            entries,
            SiteContentKeys.ContactsHours,
            request.Hours,
            dateTimeProvider.UtcNow,
            siteContentRepository);

        ContentMappings.UpsertEntry(
            entries,
            SiteContentKeys.ContactsMapEmbed,
            request.MapEmbed,
            dateTimeProvider.UtcNow,
            siteContentRepository);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new AdminSiteContentResponse(ContentMappings.MapAbout(entries), ContentMappings.MapContacts(entries));
    }
}

internal static class ContentMappings
{
    public static AboutContentDto MapAbout(IDictionary<string, SiteContentEntry> entries)
    {
        return new AboutContentDto(
            GetValue(entries, SiteContentKeys.AboutText),
            GetValue(entries, SiteContentKeys.AboutPhoto));
    }

    public static ContactsContentDto MapContacts(IDictionary<string, SiteContentEntry> entries)
    {
        return new ContactsContentDto(
            GetValue(entries, SiteContentKeys.ContactsAddress),
            GetValue(entries, SiteContentKeys.ContactsPhone),
            GetValue(entries, SiteContentKeys.ContactsHours),
            GetValue(entries, SiteContentKeys.ContactsMapEmbed));
    }

    public static void UpsertEntry(
        IDictionary<string, SiteContentEntry> entries,
        string key,
        string? value,
        DateTime utcNow,
        ISiteContentRepository siteContentRepository)
    {
        if (entries.TryGetValue(key, out var entry))
        {
            entry.UpdateValue(value, utcNow);
            return;
        }

        var newEntry = new SiteContentEntry(key, value, utcNow);
        siteContentRepository.AddRange([newEntry]);
        entries[key] = newEntry;
    }

    private static string? GetValue(IDictionary<string, SiteContentEntry> entries, string key)
    {
        return entries.TryGetValue(key, out var entry) ? entry.Value : null;
    }
}
