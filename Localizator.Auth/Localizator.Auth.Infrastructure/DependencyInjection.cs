using Localizator.Auth.Domain.Identity;
using Localizator.Auth.Domain.Interfaces.Configuration;
using Localizator.Auth.Domain.Interfaces.Strategy;
using Localizator.Auth.Infrastructure.Persistence;
using Localizator.Auth.Infrastructure.Strategies;
using Localizator.Auth.Infrastructure.Strategies.Providers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Localizator.Auth.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        AddDatabase(services, configuration);
        AddStrategies(services);
        return services;
    }
    
    private static void AddStrategies(this IServiceCollection services)
    {
        services.AddScoped<IAuthOptionsProvider, AuthOptionsProvider>();
        services.AddScoped<IAuthStrategyProvider, AuthStrategyProvider>();

        services.AddScoped<OidcAuthStrategy>();
        services.AddScoped<LocalAuthStrategy>();
        services.AddScoped<ApiKeyAuthStrategy>();
        services.AddScoped<HybridAuthStrategy>();
        services.AddScoped<NoneAuthStrategy>();

        services.AddScoped<IAuthOptions>(sp => sp.GetRequiredService<IAuthOptionsProvider>().Get());
        services.AddScoped<IAuthStrategy>(sp => sp.GetRequiredService<IAuthStrategyProvider>().Get());
    }

    private static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AuthDbContext>(options => 
            options.UseNpgsql(configuration.GetConnectionString("AuthDatabase"))
        );

        services.AddIdentity<LocalizatorIdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<AuthDbContext>()
            .AddDefaultTokenProviders();

        services.ConfigureApplicationCookie(options => {
            options.SlidingExpiration = true;
        });
    }
}
