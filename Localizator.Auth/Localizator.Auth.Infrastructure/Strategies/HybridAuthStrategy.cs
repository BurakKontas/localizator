using Localizator.Auth.Domain.Configuration.Mode;
using Localizator.Auth.Domain.Identity;
using Localizator.Auth.Domain.Interfaces.Configuration;
using Localizator.Auth.Domain.Interfaces.Strategy;
using Localizator.Auth.Infrastructure.Strategies.Abstract;
using Localizator.Shared.Result;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Localizator.Auth.Infrastructure.Strategies;

public sealed class HybridAuthStrategy(
    IAuthOptionsProvider provider,
    ILoggerFactory loggerFactory,
    UserManager<LocalizatorIdentityUser> userManager,
    SignInManager<LocalizatorIdentityUser> signInManager) : AuthStrategyBase<IHybridAuthOptions>(provider)
{
    private readonly IAuthOptionsProvider _provider = provider;
    private readonly ILogger<HybridAuthStrategy> _logger = loggerFactory.CreateLogger<HybridAuthStrategy>();
    private readonly UserManager<LocalizatorIdentityUser> _userManager = userManager;
    private readonly SignInManager<LocalizatorIdentityUser> _signInManager = signInManager;

    public override async Task<Result<int>> AuthenticateAsync(HttpContext context, CancellationToken ct = default)
    {
        if (Options is not IApiKeyAuthOptions apiKeyOptions || Options is not IOidcAuthOptions oidcAuthOptions)
        {
            _logger.LogError("HybridAuthStrategy requires both API Key and OIDC options.");
            return Result<int>.Failure("Invalid authentication configuration.", StatusCodes.Status401Unauthorized);
        }

        string message = string.Empty;

        if (apiKeyOptions is not null)
        {
            ILogger<ApiKeyAuthStrategy> apiKeyLogger = loggerFactory.CreateLogger<ApiKeyAuthStrategy>();
            var apiKeyStrategy = new ApiKeyAuthStrategy(_provider, apiKeyLogger, _userManager, _signInManager);

            Result<int> apiKeyResult = await apiKeyStrategy.AuthenticateAsync(context, ct);

            if(apiKeyResult.IsSuccess)
            {
                return apiKeyResult;
            }

            message += "API Key: " + apiKeyResult.Message + ". ";
        }

        if (oidcAuthOptions is not null)
        {
            ILogger<OidcAuthStrategy> oidcLogger = loggerFactory.CreateLogger<OidcAuthStrategy>();
            var oidcStrategy = new OidcAuthStrategy(_provider, oidcLogger, _signInManager, _userManager);

            Result<int> oidcResult = await oidcStrategy.AuthenticateAsync(context, ct);

            if(oidcResult.IsSuccess)
            {
                return oidcResult;
            }

            message += "OIDC: " + oidcResult.Message;
        }

        return Result<int>.Failure(message, StatusCodes.Status403Forbidden);
    }
}
