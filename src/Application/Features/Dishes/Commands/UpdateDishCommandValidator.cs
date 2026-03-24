using FluentValidation;
using PlovCenter.Application.Common.Validation;
using PlovCenter.Application.Contract.Dishes.Commands;

namespace PlovCenter.Application.Features.Dishes.Commands;

public sealed class UpdateDishCommandValidator : AbstractValidator<UpdateDishCommand>
{
    public UpdateDishCommandValidator()
    {
        RuleFor(static command => command.DishId).NotEmpty();
        RuleFor(static command => command.CategoryId).NotEmpty();

        RuleFor(static command => command.Name)
            .NotEmpty()
            .MaximumLength(ValidationRules.DishNameMaxLength);

        RuleFor(static command => command.Description)
            .MaximumLength(ValidationRules.DishDescriptionMaxLength);

        RuleFor(static command => command.Price)
            .GreaterThanOrEqualTo(0);

        RuleFor(static command => command.SortOrder)
            .GreaterThanOrEqualTo(0);

        RuleFor(static command => command.PhotoPath)
            .MaximumLength(512);
    }
}
