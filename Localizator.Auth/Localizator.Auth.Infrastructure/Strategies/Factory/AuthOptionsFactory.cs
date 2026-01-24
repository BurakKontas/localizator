using Localizator.Auth.Domain.Configuration;
using Localizator.Auth.Domain.Exceptions;
using Localizator.Auth.Domain.Interfaces.Configuration;
using Localizator.Auth.Domain.Interfaces.Strategy;
using Microsoft.Extensions.Configuration;

namespace Localizator.Auth.Infrastructure.Strategies.Factory;

public sealed class AuthOptionsFactory(IConfiguration config) : IAuthOptionsFactory
{
    private readonly IConfiguration _config = config;

    public IAuthOptions Create()
    {
        var mode = Enum.Parse<AuthMode>(_config["AUTH_MODE"] ?? throw new AuthConfigurationException("AUTH_MODE is required"), ignoreCase: true);

        return mode switch
        {
            AuthMode.Oidc => Bind<OidcAuthOptions>("OIDC"),
            AuthMode.Local => Bind<LocalAuthOptions>("LOCAL"),
            AuthMode.Header => Bind<HeaderAuthOptions>("HEADER"),
            AuthMode.ApiKey => Bind<ApiKeyAuthOptions>("API_KEY"),
            AuthMode.Hybrid => Bind<HybridAuthOptions>("HYBRID"),
            _ => throw new NotSupportedException()
        };
    }

    private T Bind<T>(string section) where T : IAuthOptions => _config.GetSection(section).Get<T>() ?? throw new AuthConfigurationException($"{section} auth config is missing");
}
