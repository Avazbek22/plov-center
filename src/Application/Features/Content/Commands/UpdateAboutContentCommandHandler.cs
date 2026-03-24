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

public sealed class UpdateAboutContentCommandHandler(
    IApplicationDbContext applicationDbContext,
    IDateTimeService dateTimeService) : IRequestHandler<UpdateAboutContentCommand, AdminSiteContentResponse>
{
    public async Task<AdminSiteContentResponse> Handle(UpdateAboutContentCommand request, CancellationToken cancellationToken)
    {
        var entries = await SiteContentEntriesEditor.LoadAsync(applicationDbContext, cancellationToken);
        var utcNow = dateTimeService.UtcNow;

        SiteContentEntriesEditor.Upsert(applicationDbContext, entries, SiteContentKeys.AboutText, request.Text.NormalizeOptional(), utcNow);
        SiteContentEntriesEditor.Upsert(applicationDbContext, entries, SiteContentKeys.AboutPhoto, request.PhotoPath.NormalizeOptional(), utcNow);

        await applicationDbContext.SaveChangesAsync(cancellationToken);

        return entries.ToAdminResponse();
    }
}
