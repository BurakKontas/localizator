using Localizator.Auth.Domain.Identity;
using Localizator.Auth.Domain.Interfaces.Configuration;
using Localizator.Auth.Domain.Interfaces.Strategy;
using Localizator.Auth.Infrastructure.Strategies.Abstract;
using Localizator.Shared.Resources;
using Localizator.Shared.Result;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace Localizator.Auth.Infrastructure.Strategies;

public sealed class OidcAuthStrategy(
    IAuthOptionsProvider provider,
    ILogger<OidcAuthStrategy> logger,
    SignInManager<LocalizatorIdentityUser> signInManager,
    UserManager<LocalizatorIdentityUser> userManager) : AuthStrategyBase<IOidcAuthOptions>(provider)
{
    private readonly SignInManager<LocalizatorIdentityUser> _signInManager = signInManager;
    private readonly UserManager<LocalizatorIdentityUser> _userManager = userManager;
    private readonly ILogger<OidcAuthStrategy> _logger = logger;
    private readonly JwtSecurityTokenHandler _tokenHandler = new();

    public override async Task<Result<int>> AuthenticateAsync(
        HttpContext context,
        CancellationToken ct = default
    )
    {
        if (!context.Request.Headers.TryGetValue("Authorization", out var authHeader))
            return Result<int>.Failure(Errors.AuthorizationHeaderNotFound, StatusCodes.Status401Unauthorized);

        if (!AuthenticationHeaderValue.TryParse(authHeader, out var header) ||
            header.Scheme != "Bearer")
            return Result<int>.Failure(Errors.InvalidAuthorizationScheme, StatusCodes.Status401Unauthorized);

        var token = header.Parameter;
        if (string.IsNullOrWhiteSpace(token))
            return Result<int>.Failure(Errors.BearerTokenMissing, StatusCodes.Status401Unauthorized);

        try
        {
            var configurationUrl = Options.ConfigurationUrl;

            var documentRetriever = new HttpDocumentRetriever
            {
                RequireHttps = configurationUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
            };

            var configManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                configurationUrl,
                new OpenIdConnectConfigurationRetriever(),
                documentRetriever
            );

            var oidcConfig = await configManager.GetConfigurationAsync(ct);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = Options.Issuer,

                ValidateAudience = true,
                ValidAudience = Options.Audience,

                ValidateIssuerSigningKey = true,
                IssuerSigningKeys = oidcConfig.SigningKeys,

                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(2)
            };

            ClaimsPrincipal claims = _tokenHandler.ValidateToken(token, validationParameters, out _);

            string? username =
                claims.FindFirst("preferred_username")?.Value ??
                claims.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                claims.FindFirst("sub")?.Value;

            if(username == null)
            {
                return Result<int>.Failure(Errors.UserIdentifierMissing, StatusCodes.Status401Unauthorized);
            }

            Result<bool> isLoggedIn = CheckIfUserLoggedIn(_signInManager, context, username);

            if (isLoggedIn.IsSuccess)
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
        catch (SecurityTokenException ex)
        {
            _logger.LogWarning(ex, "OIDC token validation failed.");
            return Result<int>.Failure(Errors.InvalidOIDCToken, StatusCodes.Status401Unauthorized);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OIDC authentication error.");
            return Result<int>.Failure(Errors.AnErrorOccured, StatusCodes.Status500InternalServerError);
        }
    }
}
