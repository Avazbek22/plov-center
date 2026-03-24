using FluentValidation;
using PlovCenter.Application.Contract.Dishes.Queries;

namespace PlovCenter.Application.Features.Dishes.Queries;

public sealed class GetDishByIdQueryValidator : AbstractValidator<GetDishByIdQuery>
{
    public GetDishByIdQueryValidator()
    {
        RuleFor(static query => query.DishId).NotEmpty();
    }
}
