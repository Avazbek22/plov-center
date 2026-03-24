using MediatR;
using Microsoft.EntityFrameworkCore;
using PlovCenter.Application.Common.Exceptions;
using PlovCenter.Application.Common.Interfaces.Contexts;
using PlovCenter.Application.Common.Interfaces.Services;
using PlovCenter.Application.Contract.Auth.Queries;
using PlovCenter.Application.Contract.Auth.Responses;

namespace PlovCenter.Application.Features.Auth.Queries;

public sealed class GetCurrentAdminQueryHandler(
    ICurrentUserService currentUserService,
    IApplicationDbContext applicationDbContext) : IRequestHandler<GetCurrentAdminQuery, CurrentAdminResponse>
{
    public async Task<CurrentAdminResponse> Handle(GetCurrentAdminQuery request, CancellationToken cancellationToken)
    {
        var currentUser = currentUserService.GetCurrentUser();

        if (!currentUser.IsAuthenticated || currentUser.UserId is null)
        {
            throw new UnauthorizedException("Authentication is required.");
        }

        var adminUser = await applicationDbContext.AdminUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Id == currentUser.UserId.Value, cancellationToken);

        if (adminUser is null || !adminUser.IsActive)
        {
            throw new UnauthorizedException("The current administrator is not available.");
        }

        return new CurrentAdminResponse(adminUser.Id, adminUser.Username, adminUser.IsActive);
    }
}
