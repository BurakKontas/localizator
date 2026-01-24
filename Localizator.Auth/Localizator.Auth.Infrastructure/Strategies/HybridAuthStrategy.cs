using Localizator.Auth.Domain.Configuration;
using Localizator.Auth.Domain.Interfaces.Configuration;
using Localizator.Auth.Domain.Interfaces.Strategy;
using Localizator.Auth.Infrastructure.Strategies.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Localizator.Auth.Infrastructure.Strategies;

public sealed class HybridAuthStrategy(IAuthOptionsProvider provider, ILogger<HybridAuthStrategy> logger) : AuthStrategyBase<IHybridAuthOptions>(provider)
{
    public override AuthMode Mode => AuthMode.Hybrid;
    private readonly ILogger<HybridAuthStrategy> _logger = logger;

    public override async Task AuthenticateAsync(HttpContext context, CancellationToken ct = default)
    {
        // TODO:
        // if API key present → apiKey
        // else → oidc

        _logger.LogInformation("Hybrid authentication strategy invoked.");
        _logger.LogInformation(Options.ToString());

        await Task.CompletedTask;
    }
}
