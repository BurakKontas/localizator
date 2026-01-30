using Localizator.Auth.Domain.Configuration.Mode;
using Localizator.Auth.Domain.Identity;
using Localizator.Auth.Domain.Interfaces.Configuration;
using Localizator.Auth.Domain.Interfaces.Strategy;
using Localizator.Auth.Infrastructure.Strategies.Abstract;
using Localizator.Shared.Extensions;
using Localizator.Shared.Resources;
using Localizator.Shared.Result;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Security.Claims;

namespace Localizator.Auth.Infrastructure.Strategies;

public sealed class ApiKeyAuthStrategy(
    IAuthOptionsProvider provider,
    ILogger<ApiKeyAuthStrategy> logger,
    UserManager<LocalizatorIdentityUser> userManager,
    SignInManager<LocalizatorIdentityUser> signInManager) : AuthStrategyBase<IApiKeyAuthOptions>(provider)
{
    private readonly ILogger<ApiKeyAuthStrategy> _logger = logger;
    private readonly UserManager<LocalizatorIdentityUser> _userManager = userManager;
    private readonly SignInManager<LocalizatorIdentityUser> _signInManager = signInManager;

    public override async Task<Result<int>> AuthenticateAsync(
        HttpContext context,
        CancellationToken ct = default)
    {
        var matchedKey = Options.ApiKeys
            .FirstOrDefault(kvp =>
                context.Request.Headers.TryGetValue(kvp.Key, out var values) &&
                values.FirstOrDefault() == kvp.Value
            );

        if (matchedKey.Equals(default(KeyValuePair<string, string>)))
        {
            return Result<int>.Failure(
                Errors.InvalidOrMissingAPIKey,
                StatusCodes.Status401Unauthorized
            );
        }

        var apiKeyUsed = matchedKey.Key;
        var username = apiKeyUsed;

        var isLoggedIn = CheckIfUserLoggedIn(
            _signInManager,
            context,
            username
        );

        if (isLoggedIn.IsSuccess)
        {
            return Result<int>.Success(StatusCodes.Status200OK);
        }

        await _signInManager.SignOutAsync();

        var signInResult = await SignInUserAsync(
            context,
            _signInManager,
            _userManager,
            username
        );

        return signInResult.IsSuccess
            ? Result<int>.Success(StatusCodes.Status200OK)
            : Result<int>.Failure(
                message: signInResult.Message,
                data: StatusCodes.Status401Unauthorized
            );
    }


}
