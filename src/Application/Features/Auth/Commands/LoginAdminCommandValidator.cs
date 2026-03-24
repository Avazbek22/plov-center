using FluentValidation;
using PlovCenter.Application.Common.Validation;
using PlovCenter.Application.Contract.Auth.Commands;

namespace PlovCenter.Application.Features.Auth.Commands;

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
