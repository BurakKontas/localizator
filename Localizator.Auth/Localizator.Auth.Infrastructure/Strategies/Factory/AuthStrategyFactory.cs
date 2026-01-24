using Localizator.Auth.Domain.Configuration;
using Localizator.Auth.Domain.Interfaces.Configuration;
using Localizator.Auth.Domain.Interfaces.Strategy;
using Microsoft.Extensions.DependencyInjection;

namespace Localizator.Auth.Infrastructure.Strategies.Factory;

public sealed class AuthStrategyFactory(IServiceProvider sp) : IAuthStrategyFactory
{
    private readonly IServiceProvider _sp = sp;

    public IAuthStrategy Create(IAuthOptions options)
    {
        var strategyType = options.Mode switch
        {
            AuthMode.Oidc => typeof(OidcAuthStrategy),
            AuthMode.Local => typeof(LocalAuthStrategy),
            AuthMode.Header => typeof(HeaderAuthStrategy),
            AuthMode.ApiKey => typeof(ApiKeyAuthStrategy),
            AuthMode.Hybrid => typeof(HybridAuthStrategy),
            _ => throw new NotSupportedException()
        };

        return (IAuthStrategy)ActivatorUtilities.CreateInstance(_sp, strategyType, options);
    }
}
