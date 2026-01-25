using FluentValidation;
using Localizator.Auth.Application.Interfaces.Validators;
using Localizator.Auth.Application.LocalizatorAuthorize;
using Localizator.Auth.Application.Validators;
using Localizator.Auth.Application.Validators.Resolver;
using Localizator.Auth.Domain.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Localizator.Auth.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthApplication(this IServiceCollection services)
    {
        AddOptionValidators(services);
        AddAuthAuthorization(services);
        return services;
    }

    private static void AddOptionValidators(this IServiceCollection services)
    {
        services.AddSingleton<IAuthOptionsValidatorResolver, AuthOptionsValidatorResolver>();

        services.AddSingleton<IValidator<OidcAuthOptions>, OidcAuthOptionsValidator>();
        services.AddSingleton<IValidator<LocalAuthOptions>, LocalAuthOptionsValidator>();
        services.AddSingleton<IValidator<ApiKeyAuthOptions>, ApiKeyAuthOptionsValidator>();
        services.AddSingleton<IValidator<HybridAuthOptions>, HybridAuthOptionsValidator>();
        services.AddSingleton<IValidator<NoneAuthOptions>, NoneAuthOptionsValidator>();
    }

    private static void AddAuthAuthorization(this IServiceCollection services)
    {
        services.AddAuthorizationCore(options =>
        {
            options.AddPolicy("LocalizatorPolicy", policy =>
                policy.Requirements.Add(new LocalizatorRequirement())
            );
        });

        services.AddScoped<IAuthorizationHandler, LocalizatorHandler>();
    }
}
