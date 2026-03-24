using FluentValidation;
using PlovCenter.Application.Contract.Categories.Commands;

namespace PlovCenter.Application.Features.Categories.Commands;

public sealed class ReorderCategoriesCommandValidator : AbstractValidator<ReorderCategoriesCommand>
{
    public ReorderCategoriesCommandValidator()
    {
        RuleFor(static command => command.Items).NotEmpty();

        RuleForEach(static command => command.Items)
            .ChildRules(item =>
            {
                item.RuleFor(static value => value.CategoryId).NotEmpty();
                item.RuleFor(static value => value.SortOrder).GreaterThanOrEqualTo(0);
            });
    }
}
