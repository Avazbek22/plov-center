using MediatR;
using Microsoft.EntityFrameworkCore;
using PlovCenter.Application.Common.Exceptions;
using PlovCenter.Application.Common.Interfaces.Contexts;
using PlovCenter.Application.Common.Interfaces.Services;
using PlovCenter.Application.Contract.Auth.Commands;
using PlovCenter.Application.Contract.Auth.Responses;

namespace PlovCenter.Application.Features.Auth.Commands;

public sealed class LoginAdminCommandHandler(
    IApplicationDbContext applicationDbContext,
    IPasswordHashService passwordHashService,
    IJwtTokenService jwtTokenService) : IRequestHandler<LoginAdminCommand, LoginResponse>
{
    public async Task<LoginResponse> Handle(LoginAdminCommand request, CancellationToken cancellationToken)
    {
        var normalizedUsername = request.Username.Trim().ToLowerInvariant();

        var adminUser = await applicationDbContext.AdminUsers
            .FirstOrDefaultAsync(user => user.Username == normalizedUsername, cancellationToken);

        if (adminUser is null || !adminUser.IsActive || !passwordHashService.VerifyPassword(adminUser, request.Password))
        {
            throw new UnauthorizedException("Invalid username or password.");
        }

        var token = jwtTokenService.Create(adminUser);
        return new LoginResponse(token.Token, token.ExpiresAtUtc, new CurrentAdminResponse(adminUser.Id, adminUser.Username, adminUser.IsActive));
    }
}
