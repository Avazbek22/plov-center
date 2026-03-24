using MediatR;
using Microsoft.EntityFrameworkCore;
using PlovCenter.Application.Common.Constants;
using PlovCenter.Application.Common.Interfaces.Contexts;
using PlovCenter.Application.Contract.Content.Queries;
using PlovCenter.Application.Contract.Content.Responses;
using PlovCenter.Application.Features.Content.Mappings;

namespace PlovCenter.Application.Features.Content.Queries;

public sealed class GetAdminSiteContentQueryHandler(IApplicationDbContext applicationDbContext)
    : IRequestHandler<GetAdminSiteContentQuery, AdminSiteContentResponse>
{
    public async Task<AdminSiteContentResponse> Handle(GetAdminSiteContentQuery request, CancellationToken cancellationToken)
    {
        var entries = await applicationDbContext.SiteContentEntries
            .AsNoTracking()
            .Where(entry => SiteContentKeys.All.Contains(entry.Key))
            .ToDictionaryAsync(entry => entry.Key, cancellationToken);

        return entries.ToAdminResponse();
    }
}
