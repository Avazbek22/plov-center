using FluentValidation;
using PlovCenter.Application.Common.Validation;
using PlovCenter.Application.Contract.Categories.Commands;

namespace PlovCenter.Application.Features.Categories.Commands;

public sealed class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(static command => command.Name)
            .NotEmpty()
            .MaximumLength(ValidationRules.CategoryNameMaxLength);

        RuleFor(static command => command.SortOrder)
            .GreaterThanOrEqualTo(0);
    }
}
