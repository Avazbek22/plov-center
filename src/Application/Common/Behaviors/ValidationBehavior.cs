using FluentValidation;
using MediatR;
using PlovCenter.Application.Common.Exceptions;

namespace PlovCenter.Application.Common.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                validators.Select(validator => validator.ValidateAsync(context, cancellationToken)));

            var errors = validationResults
                .SelectMany(static result => result.Errors)
                .Where(static failure => failure is not null)
                .GroupBy(static failure => failure.PropertyName)
                .ToDictionary(
                    static group => group.Key,
                    static group => group.Select(static failure => failure.ErrorMessage).Distinct().ToArray());

            if (errors.Count > 0)
            {
                throw new RequestValidationException(errors);
            }
        }

        return await next();
    }
}
