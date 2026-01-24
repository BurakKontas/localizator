using Localizator.Auth.Domain.Configuration;
using Localizator.Auth.Domain.Interfaces.Configuration;
using Localizator.Auth.Domain.Interfaces.Strategy;
using Localizator.Auth.Infrastructure.Strategies.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Localizator.Auth.Infrastructure.Strategies;

public sealed class OidcAuthStrategy(IAuthOptionsProvider provider, ILogger<OidcAuthStrategy> logger) : AuthStrategyBase<IOidcAuthOptions>(provider)
{
    public override AuthMode Mode => AuthMode.Oidc;
    private readonly ILogger<OidcAuthStrategy> _logger = logger;

    public override Task AuthenticateAsync(HttpContext context, CancellationToken ct = default)
    {
        // TODO:
        // - challenge redirect
        // - callback handling
        // - token validation

        _logger.LogInformation("OIDC authentication strategy invoked.");
        _logger.LogInformation(Options.ToString());

        return Task.CompletedTask;
    }
}