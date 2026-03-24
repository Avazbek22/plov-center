using FluentValidation;
using PlovCenter.Application.Contract.Categories.Queries;

namespace PlovCenter.Application.Features.Categories.Queries;

public sealed class GetCategoryByIdQueryValidator : AbstractValidator<GetCategoryByIdQuery>
{
    public GetCategoryByIdQueryValidator()
    {
        RuleFor(static query => query.CategoryId).NotEmpty();
    }
}
