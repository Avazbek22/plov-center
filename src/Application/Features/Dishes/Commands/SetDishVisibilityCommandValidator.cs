using FluentValidation;
using PlovCenter.Application.Contract.Dishes.Commands;

namespace PlovCenter.Application.Features.Dishes.Commands;

public sealed class SetDishVisibilityCommandValidator : AbstractValidator<SetDishVisibilityCommand>
{
    public SetDishVisibilityCommandValidator()
    {
        RuleFor(static command => command.DishId).NotEmpty();
    }
}
