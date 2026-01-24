using Localizator.Auth.Domain.Interfaces.Configuration;
using Localizator.Auth.Domain.Interfaces.Strategy;
using Localizator.Auth.Infrastructure.Strategies;
using Localizator.Auth.Infrastructure.Strategies.Factory;
using Localizator.Auth.Infrastructure.Strategies.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace Localizator.Auth.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthInfrastructure(this IServiceCollection services)
    {
        AddStrategies(services);
        return services;
    }
    
    private static void AddStrategies(this IServiceCollection services)
    {
        services.AddSingleton<IAuthOptionsProvider, AuthOptionsProvider>();
        services.AddSingleton<IAuthOptionsFactory, AuthOptionsFactory>();
        services.AddSingleton<IAuthStrategyProvider, AuthStrategyProvider>();

        services.AddSingleton<OidcAuthStrategy>();
        services.AddSingleton<LocalAuthStrategy>();
        services.AddSingleton<HeaderAuthStrategy>();
        services.AddSingleton<ApiKeyAuthStrategy>();
        services.AddSingleton<HybridAuthStrategy>();

        services.AddSingleton<IAuthOptions>(sp => sp.GetRequiredService<IAuthOptionsProvider>().Get());
        services.AddSingleton<IAuthStrategy>(sp => sp.GetRequiredService<IAuthStrategyProvider>().Get());
    }
}
