using FluentValidation;
using PlovCenter.Application.Common.Validation;
using PlovCenter.Application.Contract.Dishes;
using PlovCenter.Application.Contract.Dishes.Commands;

namespace PlovCenter.Application.Features.Dishes.Commands;

public sealed class CreateDishCommandValidator : AbstractValidator<CreateDishCommand>
{
    public CreateDishCommandValidator()
    {
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

        RuleFor(static command => command.Photos)
            .NotNull()
            .Must(static photos => photos.Select(p => p.SortOrder).Distinct().Count() == photos.Count)
            .WithMessage("Photos must not contain duplicate sort orders.")
            .Must(static photos => photos.Count <= 50)
            .WithMessage("A dish may have at most 50 photos.");

        RuleForEach(static command => command.Photos).ChildRules(photo =>
        {
            photo.RuleFor(static p => p.RelativePath)
                .NotEmpty()
                .MaximumLength(ValidationRules.DishPhotoRelativePathMaxLength);

            photo.RuleFor(static p => p.SortOrder)
                .GreaterThanOrEqualTo(0);
        });
    }
}
