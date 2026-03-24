using FluentValidation;
using PlovCenter.Application.Common.Validation;
using PlovCenter.Application.Contract.Content.Commands;

namespace PlovCenter.Application.Features.Content.Commands;

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
