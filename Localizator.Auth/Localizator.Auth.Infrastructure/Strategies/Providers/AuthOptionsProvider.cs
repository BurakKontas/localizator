using Localizator.Auth.Domain.Configuration;
using Localizator.Auth.Domain.Exceptions;
using Localizator.Auth.Domain.Interfaces.Configuration;
using Localizator.Auth.Domain.Interfaces.Strategy;
using Microsoft.Extensions.Configuration;

namespace Localizator.Auth.Infrastructure.Strategies.Providers;

public sealed class AuthOptionsProvider(IConfiguration config) : IAuthOptionsProvider
{
    private readonly IConfiguration _config = config;

    private IAuthOptions? _cached;

    public IAuthOptions Get()
    {
        if (_cached is not null)
            return _cached;

        var mode = _config.GetValue<string>("AUTH_MODE") ?? throw new AuthConfigurationException("AUTH_MODE is required");

        _cached = mode.ToLowerInvariant() switch
        {
            "oidc" => BindAndValidate<OidcAuthOptions>("OIDC"),
            "local" => BindAndValidate<LocalAuthOptions>("LOCAL"),
            "apikey" => BindAndValidate<ApiKeyAuthOptions>("API_KEY"),
            "hybrid" => BindAndValidate<HybridAuthOptions>("HYBRID"),
            "none" => BindAndValidate<NoneAuthOptions>("NONE"),
            _ => throw new AuthConfigurationException($"Unsupported AUTH_MODE: {mode}")
        };

        return _cached;
    }

    private T BindAndValidate<T>(string section)
        where T : IAuthOptions
    {
        if(section == "NONE") 
        {
            // For NONE auth mode, we return a default instance without binding
            return Activator.CreateInstance<T>();
        }  

        return _config.GetSection(section).Get<T>() ?? throw new AuthConfigurationException($"{section} auth config is missing");
    }
}
