using FluentValidation;
using PlovCenter.Application.Contract.Categories.Commands;

namespace PlovCenter.Application.Features.Categories.Commands;

public sealed class SetCategoryVisibilityCommandValidator : AbstractValidator<SetCategoryVisibilityCommand>
{
    public SetCategoryVisibilityCommandValidator()
    {
        RuleFor(static command => command.CategoryId).NotEmpty();
    }
}
