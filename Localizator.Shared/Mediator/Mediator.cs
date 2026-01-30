using Localizator.Shared.Exceptions;
using Localizator.Shared.Extensions;
using Localizator.Shared.Mediator.Interfaces;
using Localizator.Shared.Resources;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Localizator.Shared.Mediator;

public class Mediator(IServiceProvider serviceProvider) : IMediator
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public async Task<TResponse> Send<TResponse>(IRequest request, CancellationToken cancellationToken = default)
    {
        var requestType = request.GetType();
        var responseType = typeof(TResponse);

        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);

        var handler = _serviceProvider.GetService(handlerType);

        if (handler == null)
        {
            throw new TechnicalException(Errors.HandlerMethodNotFound.Format(requestType.Name));
        }

        var behaviorType = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, responseType);
        var behaviors = _serviceProvider.GetServices(behaviorType).Reverse().ToList();

        RequestHandlerDelegate<TResponse> handlerDelegate = async () =>
        {
            var handleMethod = handlerType.GetMethod("Handle");
            Task<TResponse>? result = handleMethod?.Invoke(handler, [request, cancellationToken]) as Task<TResponse>;

            if (result is null) return default!;

            return await result;
        };

        foreach (var behavior in behaviors)
        {
            var currentDelegate = handlerDelegate;
            var behaviorHandleMethod = behaviorType.GetMethod("Handle");

            handlerDelegate = () =>
            {
                Task<TResponse>? result = behaviorHandleMethod?.Invoke(behavior, [request, currentDelegate, cancellationToken]) as Task<TResponse>;

                if (result is null) return default!;

                return result;
            };
        }

        return await handlerDelegate();
    }

    public Task Send(IRequest request, CancellationToken cancellationToken = default)
    {
        return Send<object>(request, cancellationToken);
    }
}
