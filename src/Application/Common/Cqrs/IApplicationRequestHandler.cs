namespace PlovCenter.Application.Common.Cqrs;

public interface IApplicationRequestHandler<in TRequest, TResponse>
    where TRequest : IApplicationRequest<TResponse>
{
    Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken);
}
