using FluentValidation;
using Localizator.Auth.Application.Interfaces.Validators;
using Localizator.Auth.Application.Validators;
using Localizator.Auth.Application.Validators.Resolver;
using Localizator.Auth.Domain.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Localizator.Auth.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthApplication(this IServiceCollection services)
    {
        AddOptionValidators(services);
        return services;
    }

    private static void AddOptionValidators(this IServiceCollection services)
    {
        services.AddSingleton<IAuthOptionsValidatorResolver, AuthOptionsValidatorResolver>();

        services.AddSingleton<IValidator<OidcAuthOptions>, OidcAuthOptionsValidator>();
        services.AddSingleton<IValidator<LocalAuthOptions>, LocalAuthOptionsValidator>();
        services.AddSingleton<IValidator<HeaderAuthOptions>, HeaderAuthOptionsValidator>();
        services.AddSingleton<IValidator<ApiKeyAuthOptions>, ApiKeyAuthOptionsValidator>();
        services.AddSingleton<IValidator<HybridAuthOptions>, HybridAuthOptionsValidator>();
    }
}
