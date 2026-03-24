using MediatR;
using PlovCenter.Application.Common.Extensions;
using PlovCenter.Application.Common.Constants;
using PlovCenter.Application.Common.Interfaces.Contexts;
using PlovCenter.Application.Common.Interfaces.Services;
using PlovCenter.Application.Contract.Content.Commands;
using PlovCenter.Application.Contract.Content.Responses;
using PlovCenter.Application.Features.Content.Helpers;
using PlovCenter.Application.Features.Content.Mappings;

namespace PlovCenter.Application.Features.Content.Commands;

public sealed class UpdateContactsContentCommandHandler(
    IApplicationDbContext applicationDbContext,
    IDateTimeService dateTimeService) : IRequestHandler<UpdateContactsContentCommand, AdminSiteContentResponse>
{
    public async Task<AdminSiteContentResponse> Handle(UpdateContactsContentCommand request, CancellationToken cancellationToken)
    {
        var entries = await SiteContentEntriesEditor.LoadAsync(applicationDbContext, cancellationToken);
        var utcNow = dateTimeService.UtcNow;

        SiteContentEntriesEditor.Upsert(applicationDbContext, entries, SiteContentKeys.ContactsAddress, request.Address.NormalizeOptional(), utcNow);
        SiteContentEntriesEditor.Upsert(applicationDbContext, entries, SiteContentKeys.ContactsPhone, request.Phone.NormalizeOptional(), utcNow);
        SiteContentEntriesEditor.Upsert(applicationDbContext, entries, SiteContentKeys.ContactsHours, request.Hours.NormalizeOptional(), utcNow);
        SiteContentEntriesEditor.Upsert(applicationDbContext, entries, SiteContentKeys.ContactsMapEmbed, request.MapEmbed.NormalizeOptional(), utcNow);

        await applicationDbContext.SaveChangesAsync(cancellationToken);

        return entries.ToAdminResponse();
    }
}
