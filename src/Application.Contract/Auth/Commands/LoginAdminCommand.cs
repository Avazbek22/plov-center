using MediatR;
using PlovCenter.Application.Contract.Auth.Responses;

namespace PlovCenter.Application.Contract.Auth.Commands;

public sealed class LoginAdminCommand : IRequest<LoginResponse>
{
    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}
