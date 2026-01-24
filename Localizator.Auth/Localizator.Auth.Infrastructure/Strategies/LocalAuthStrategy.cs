using Localizator.Auth.Domain.Configuration;
using Localizator.Auth.Domain.Interfaces.Configuration;
using Localizator.Auth.Domain.Interfaces.Strategy;
using Localizator.Auth.Infrastructure.Strategies.Abstract;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace Localizator.Auth.Infrastructure.Strategies;

public sealed class LocalAuthStrategy(IAuthOptionsProvider provider, ILogger<LocalAuthStrategy> logger) : AuthStrategyBase<ILocalAuthOptions>(provider)
{
    public override AuthMode Mode => AuthMode.Local;
    private readonly ILogger<LocalAuthStrategy> _logger = logger;

    public override Task AuthenticateAsync(HttpContext context, CancellationToken ct = default)
    {
        // TODO:
        // - basic credential check
        // - session creation

        _logger.LogInformation("Local authentication strategy invoked.");
        _logger.LogInformation(Options.ToString());

        return Task.CompletedTask;
    }
}
