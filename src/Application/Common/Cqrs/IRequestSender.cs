namespace PlovCenter.Application.Common.Cqrs;

public interface IRequestSender
{
    Task<TResponse> SendAsync<TResponse>(IApplicationRequest<TResponse> request, CancellationToken cancellationToken = default);
}
