using FluentValidation;
using PlovCenter.Application.Abstractions.Auth;
using PlovCenter.Application.Abstractions.Persistence;
using PlovCenter.Application.Abstractions.Services;
using PlovCenter.Application.Common.Cqrs;
using PlovCenter.Application.Common.Exceptions;
using PlovCenter.Application.Common.Validation;
using PlovCenter.Application.Contract.Auth;

namespace PlovCenter.Application.Features.Auth;

public sealed record LoginAdminCommand(string Username, string Password) : IApplicationRequest<LoginResponse>;

public sealed record GetCurrentAdminQuery() : IApplicationRequest<CurrentAdminResponse>;

public sealed class LoginAdminCommandValidator : AbstractValidator<LoginAdminCommand>
{
    public LoginAdminCommandValidator()
    {
        RuleFor(static command => command.Username)
            .NotEmpty()
            .MaximumLength(ValidationRules.UsernameMaxLength);

        RuleFor(static command => command.Password)
            .NotEmpty();
    }
}

internal sealed class LoginAdminCommandHandler(
    IAdminUserRepository adminUserRepository,
    IPasswordHashService passwordHashService,
    IAdminTokenService adminTokenService) : IApplicationRequestHandler<LoginAdminCommand, LoginResponse>
{
    public async Task<LoginResponse> HandleAsync(LoginAdminCommand request, CancellationToken cancellationToken)
    {
        var adminUser = await adminUserRepository.GetByUsernameAsync(request.Username, cancellationToken);

        if (adminUser is null || !adminUser.IsActive || !passwordHashService.VerifyPassword(adminUser, request.Password))
        {
            throw new UnauthorizedException("Invalid username or password.");
        }

        var token = adminTokenService.CreateToken(adminUser);
        var adminResponse = new CurrentAdminResponse(adminUser.Id, adminUser.Username, adminUser.IsActive);

        return new LoginResponse(token.Token, token.ExpiresAtUtc, adminResponse);
    }
}

internal sealed class GetCurrentAdminQueryHandler(
    ICurrentUserContextAccessor currentUserContextAccessor,
    IAdminUserRepository adminUserRepository) : IApplicationRequestHandler<GetCurrentAdminQuery, CurrentAdminResponse>
{
    public async Task<CurrentAdminResponse> HandleAsync(GetCurrentAdminQuery request, CancellationToken cancellationToken)
    {
        var currentUser = currentUserContextAccessor.GetCurrentUser();

        if (!currentUser.IsAuthenticated || currentUser.UserId is null)
        {
            throw new UnauthorizedException("Authentication is required.");
        }

        var adminUser = await adminUserRepository.GetByIdAsync(currentUser.UserId.Value, cancellationToken);

        if (adminUser is null || !adminUser.IsActive)
        {
            throw new UnauthorizedException("The current administrator is not available.");
        }

        return new CurrentAdminResponse(adminUser.Id, adminUser.Username, adminUser.IsActive);
    }
}
