using Localizator.Auth.Domain.Configuration;
using Localizator.Auth.Domain.Interfaces.Configuration;
using Localizator.Auth.Domain.Interfaces.Strategy;
using Localizator.Auth.Infrastructure.Strategies.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Localizator.Auth.Infrastructure.Strategies;

public sealed class HeaderAuthStrategy(IAuthOptionsProvider provider, ILogger<HeaderAuthStrategy> logger) : AuthStrategyBase<IHeaderAuthOptions>(provider)
{
    public override AuthMode Mode => AuthMode.Header;
    private readonly ILogger<HeaderAuthStrategy> _logger = logger;

    public override Task AuthenticateAsync(HttpContext context, CancellationToken ct = default)
    {
        // TODO:
        // - read trusted headers
        // - map user identity

        _logger.LogInformation("Header authentication strategy invoked.");
        _logger.LogInformation(Options.ToString());

        return Task.CompletedTask;
    }
}
