using Localizator.Auth.Domain.Configuration;
using Localizator.Auth.Domain.Interfaces.Strategy;
using Microsoft.Extensions.DependencyInjection;

namespace Localizator.Auth.Infrastructure.Strategies.Providers;

public sealed class AuthStrategyProvider(IAuthOptionsProvider optionsProvider, IServiceProvider sp) : IAuthStrategyProvider
{
    private readonly IAuthOptionsProvider _optionsProvider = optionsProvider;
    private readonly IServiceProvider _sp = sp;

    private IAuthStrategy? _cached;

    public IAuthStrategy Get()
    {
        if (_cached is not null)
            return _cached;

        var options = _optionsProvider.Get();

        _cached = options.Mode switch
        {
            AuthMode.Oidc => _sp.GetRequiredService<OidcAuthStrategy>(),
            AuthMode.Local => _sp.GetRequiredService<LocalAuthStrategy>(),
            AuthMode.Header => _sp.GetRequiredService<HeaderAuthStrategy>(),
            AuthMode.ApiKey => _sp.GetRequiredService<ApiKeyAuthStrategy>(),
            AuthMode.Hybrid => _sp.GetRequiredService<HybridAuthStrategy>(),
            _ => throw new NotSupportedException()
        };

        return _cached;
    }
}
