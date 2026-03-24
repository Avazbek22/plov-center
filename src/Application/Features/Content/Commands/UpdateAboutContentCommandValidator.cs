using FluentValidation;
using PlovCenter.Application.Common.Validation;
using PlovCenter.Application.Contract.Content.Commands;

namespace PlovCenter.Application.Features.Content.Commands;

public sealed class UpdateAboutContentCommandValidator : AbstractValidator<UpdateAboutContentCommand>
{
    public UpdateAboutContentCommandValidator()
    {
        RuleFor(static command => command.Text).MaximumLength(ValidationRules.ContentValueMaxLength);
        RuleFor(static command => command.PhotoPath).MaximumLength(512);
    }
}
