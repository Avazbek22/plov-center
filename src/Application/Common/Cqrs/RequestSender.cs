using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
using PlovCenter.Application.Common.Exceptions;

namespace PlovCenter.Application.Common.Cqrs;

internal sealed class RequestSender(IServiceProvider serviceProvider) : IRequestSender
{
    public async Task<TResponse> SendAsync<TResponse>(
        IApplicationRequest<TResponse> request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        await ValidateAsync(request, cancellationToken);

        var handlerType = typeof(IApplicationRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));
        var handler = serviceProvider.GetRequiredService(handlerType);

        // The dispatcher resolves handlers by closed generic type so controllers stay unaware of concrete use cases.
        return await HandleAsync<TResponse>((dynamic)handler, (dynamic)request, cancellationToken);
    }

    private async Task ValidateAsync<TResponse>(IApplicationRequest<TResponse> request, CancellationToken cancellationToken)
    {
        var requestType = request.GetType();
        var validatorType = typeof(IValidator<>).MakeGenericType(requestType);
        var validators = serviceProvider.GetServices(validatorType).ToArray();

        if (validators.Length == 0)
        {
            return;
        }

        var validationContext = Activator.CreateInstance(typeof(ValidationContext<>).MakeGenericType(requestType), request);

        if (validationContext is null)
        {
            throw new InvalidOperationException($"Could not create validation context for {requestType.Name}.");
        }

        var results = new List<ValidationResult>(validators.Length);

        foreach (var validator in validators)
        {
            results.Add(await ValidateAsync((dynamic)validator, (dynamic)validationContext, cancellationToken));
        }

        var errors = results
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

    private static Task<ValidationResult> ValidateAsync(
        dynamic validator,
        dynamic validationContext,
        CancellationToken cancellationToken)
    {
        return validator.ValidateAsync(validationContext, cancellationToken);
    }

    private static Task<TResponse> HandleAsync<TResponse>(
        dynamic handler,
        dynamic request,
        CancellationToken cancellationToken)
    {
        return handler.HandleAsync(request, cancellationToken);
    }
}
