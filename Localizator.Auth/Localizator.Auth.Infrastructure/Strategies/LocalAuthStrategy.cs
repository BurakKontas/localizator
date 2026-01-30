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
using System.Security.Claims;
using System.Text;

namespace Localizator.Auth.Infrastructure.Strategies;

public sealed class LocalAuthStrategy(
    IAuthOptionsProvider provider,
    ILogger<LocalAuthStrategy> logger,
    SignInManager<LocalizatorIdentityUser> signInManager,
    UserManager<LocalizatorIdentityUser> userManager) : AuthStrategyBase<ILocalAuthOptions>(provider)
{
    private readonly ILogger<LocalAuthStrategy> _logger = logger;
    private readonly SignInManager<LocalizatorIdentityUser> _signInManager = signInManager;
    private readonly UserManager<LocalizatorIdentityUser> _userManager = userManager;

    public override async Task<Result<int>> AuthenticateAsync(HttpContext context, CancellationToken ct = default)
    {
        if (!context.Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            return Result<int>.Failure(Errors.AuthorizationHeaderNotFound, StatusCodes.Status401Unauthorized);
        }

        var headerValue = authHeader.ToString();
        if (!headerValue.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
        {
            return Result<int>.Failure(Errors.BasicAuthorizationHeaderInvalidFormat, StatusCodes.Status401Unauthorized);
        }

        // Decode Base64 credentials
        var encodedCredentials = headerValue["Basic ".Length..].Trim();
        string decoded;
        try
        {
            var bytes = Convert.FromBase64String(encodedCredentials);
            decoded = Encoding.UTF8.GetString(bytes);
        }
        catch
        {
            return Result<int>.Failure(Errors.Base64ConversionError, StatusCodes.Status401Unauthorized);
        }

        var parts = decoded.Split(':', 2);
        if (parts.Length != 2)
        {
            return Result<int>.Failure(data: StatusCodes.Status401Unauthorized);
        }

        var (username, password) = (parts[0], parts[1]);

        // Basic credential check
        if (username != Options.AdminUser || password != Options.AdminPassword)
        {
            return Result<int>.Failure(Errors.BasicCredentialsDontMatch, StatusCodes.Status401Unauthorized);
        }

        Result<bool> isLoggedIn = CheckIfUserLoggedIn(_signInManager, context, username);

        if(isLoggedIn.IsSuccess)
        {
            return Result<int>.Success(StatusCodes.Status200OK);
        }
        else
        {
            await _signInManager.SignOutAsync();
        }

        return await SignInUserAsync(
            context,
            _signInManager,
            _userManager,
            username);
    }
}
