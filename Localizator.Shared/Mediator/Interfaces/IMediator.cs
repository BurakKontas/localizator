namespace Localizator.Shared.Mediator.Interfaces;

public interface IMediator
{
    Task<TResponse> Send<TResponse>(IRequest request, CancellationToken cancellationToken = default);
    Task Send(IRequest request, CancellationToken cancellationToken = default);
}
