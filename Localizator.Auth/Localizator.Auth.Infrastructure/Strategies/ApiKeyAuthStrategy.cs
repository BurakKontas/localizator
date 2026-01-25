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

    public override async Task<Result<bool>> AuthenticateAsync(
        HttpContext context,
        CancellationToken ct = default)
    {
        var matchedKey = Options.ApiKeys
            .FirstOrDefault(kvp =>
                context.Request.Headers.TryGetValue(kvp.Key, out var values) &&
                values.FirstOrDefault() == kvp.Value
            );

        if (matchedKey.Equals(default(KeyValuePair<string, string>)))
            return Result<bool>.Failure(Errors.InvalidOrMissingAPIKey);

        var apiKeyUsed = matchedKey.Key;

        string username = apiKeyUsed;

        Result<bool> isLoggedIn = CheckIfUserLoggedIn(_signInManager, context, username);

        if (isLoggedIn.IsSuccess && isLoggedIn.Data)
        {
            return Result<bool>.Success(true);
        }
        else
        {
            await _signInManager.SignOutAsync();
        }

        var user = await _userManager.FindByNameAsync(username);
        if (user == null)
        {
            user = new LocalizatorIdentityUser(username, Options.Mode);
            var result = await _userManager.CreateAsync(user);

            if (!result.Succeeded)
            {
                string message = Errors.FailedToCreateIdentityUser.Format(string.Join(", ", result.Errors.Select(e => e.Description)));

                _logger.LogError(message);
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                return Result<bool>.Failure(message);
            }
        }

        var principal = await _signInManager.CreateUserPrincipalAsync(user);

        principal.Identities.First().AddClaim(
            new Claim("auth_mode", Options.Mode.ToString())
        );

        principal.Identities.First().AddClaim(
            new Claim(ClaimTypes.AuthenticationMethod, "apikey")
        );

        await context.SignInAsync(
            IdentityConstants.ApplicationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = false
            });

        return Result<bool>.Success(true);
    }

}
