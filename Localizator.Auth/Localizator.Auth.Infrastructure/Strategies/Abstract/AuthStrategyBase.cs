using Localizator.Auth.Domain.Configuration.Mode;
using Localizator.Auth.Domain.Identity;
using Localizator.Auth.Domain.Interfaces.Configuration;
using Localizator.Auth.Domain.Interfaces.Strategy;
using Localizator.Shared.Extensions;
using Localizator.Shared.Resources;
using Localizator.Shared.Result;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Localizator.Auth.Infrastructure.Strategies.Abstract;

public abstract class AuthStrategyBase<TOptions>(IAuthOptionsProvider provider) : IAuthStrategy where TOptions : IAuthOptions
{
    protected TOptions Options { get; } = (TOptions) provider.Get();
    public AuthMode Mode { get; init; } = provider.Get().Mode;
    public abstract Task<Result<int>> AuthenticateAsync(HttpContext context, CancellationToken ct = default);

    protected Result<bool> CheckIfUserLoggedIn(SignInManager<LocalizatorIdentityUser> signInManager, HttpContext context, string username)
    {
        if (signInManager.IsSignedIn(context.User) && context.User?.Identity?.Name == username)
        {
            var authMode = context.User.FindFirst("auth_mode")?.Value;

            if (authMode == Options.Mode.ToString())
            {
                return Result<bool>.Success();
            }

            return Result<bool>.Failure();
        }

        return Result<bool>.Failure();
    }

    protected async Task<LocalizatorIdentityUser> GetOrCreateUserAsync(UserManager<LocalizatorIdentityUser> userManager, string username, string? password)
    {
        var user = await userManager.FindByNameAsync(username);
        if (user != null)
            return user;

        user = new LocalizatorIdentityUser(username, Options.Mode);

        IdentityResult result = password is not null
            ? await userManager.CreateAsync(user, password)
            : await userManager.CreateAsync(user);

        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                Errors.FailedToCreateIdentityUser.Format(
                    string.Join(", ", result.Errors.Select(e => e.Description)))
            );
        }

        return user;
    }

    protected async Task<ClaimsPrincipal> CreatePrincipalAsync(SignInManager<LocalizatorIdentityUser> signInManager, LocalizatorIdentityUser user)
    {
        return await signInManager.CreateUserPrincipalAsync(user);
    }

    protected void AddBaseClaims(ClaimsIdentity identity, string username)
    {
        identity.AddClaim(new Claim("auth_mode", Options.Mode.ToString()));
        identity.AddClaim(new Claim(ClaimTypes.AuthenticationMethod, Options.Mode.ToString()));
        identity.AddClaim(new Claim(ClaimTypes.Name, username));
    }

    protected void AddExtraClaims(ClaimsIdentity identity, IEnumerable<Claim>? extraClaims)
    {
        if (extraClaims == null)
            return;

        foreach (var claim in extraClaims)
            identity.AddClaim(claim);
    }

    protected async Task SignInAsync(HttpContext context, ClaimsPrincipal principal)
    {
        await context.SignInAsync(
            IdentityConstants.ApplicationScheme,
            principal,
            new AuthenticationProperties { IsPersistent = false });

        context.User = principal;
    }

    protected async Task<Result<int>> SignInUserAsync(
        HttpContext context,
        SignInManager<LocalizatorIdentityUser> signInManager,
        UserManager<LocalizatorIdentityUser> userManager,
        string username,
        string? password = null,
        IEnumerable<Claim>? extraClaims = null)
    {
        try
        {
            var user = await GetOrCreateUserAsync(userManager, username, password);

            var principal = await CreatePrincipalAsync(signInManager, user);
            var identity = principal.Identities.First();

            AddBaseClaims(identity, username);
            AddExtraClaims(identity, extraClaims);

            await SignInAsync(context, principal);

            return Result<int>.Success(StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return Result<int>.Failure(ex.Message, StatusCodes.Status500InternalServerError);
        }
    }
}
