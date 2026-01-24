using Localizator.Auth.Domain.Configuration;
using Localizator.Auth.Domain.Interfaces.Configuration;
using Localizator.Auth.Domain.Interfaces.Strategy;
using Localizator.Auth.Infrastructure.Strategies.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Localizator.Auth.Infrastructure.Strategies;

public sealed class ApiKeyAuthStrategy(IAuthOptionsProvider provider, ILogger<ApiKeyAuthStrategy> logger) : AuthStrategyBase<IApiKeyAuthOptions>(provider)
{
    public override AuthMode Mode => AuthMode.ApiKey;
    private readonly ILogger<ApiKeyAuthStrategy> _logger = logger;

    public override Task AuthenticateAsync(HttpContext context, CancellationToken ct = default)
    {
        // TODO:
        // - read API key from header
        // - scope validation

        _logger.LogInformation("ApiKey authentication strategy invoked.");
        _logger.LogInformation(Options.ToString());

        return Task.CompletedTask;
    }
}
