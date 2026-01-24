using FluentValidation;
using Localizator.Auth.Application.Interfaces.Validators;
using Localizator.Auth.Domain.Configuration;
using Localizator.Auth.Domain.Exceptions;
using Localizator.Auth.Domain.Interfaces.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Localizator.Auth.Application.Validators.Resolver;

public sealed class AuthOptionsValidatorResolver(IServiceProvider sp, ILogger<AuthOptionsValidatorResolver> logger) : IAuthOptionsValidatorResolver
{
    private readonly IServiceProvider _sp = sp;
    private readonly ILogger<AuthOptionsValidatorResolver> _logger = logger;

    public void Validate(IAuthOptions options)
    {
        var optionsType = options.GetType();

        _logger.LogInformation("Resolving auth options validator for {OptionsType}", optionsType.Name);

        var validateMethod = typeof(AuthOptionsValidatorResolver)
            .GetMethod(nameof(ValidateInternal), BindingFlags.NonPublic | BindingFlags.Instance)!
            .MakeGenericMethod(optionsType);

        validateMethod.Invoke(this, [options]);
    }

    private void ValidateInternal<T>(T options)
    {
        var validator = _sp.GetRequiredService<IValidator<T>>();

        _logger.LogInformation("Running validator {ValidatorType} for {OptionsType}", validator.GetType().Name, typeof(T).Name);

        var result = validator.Validate(options);

        if (!result.IsValid)
        {
            var errors = string.Join(
                Environment.NewLine,
                result.Errors.Select(e => e.ErrorMessage));

            throw new AuthConfigurationException(errors);
        }

        _logger.LogInformation("Auth options validation PASSED for {OptionsType}", typeof(T).Name);
    }
}
