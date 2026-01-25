using Localizator.Auth.Domain.Configuration.Mode;
using Localizator.Auth.Domain.Identity;
using Localizator.Auth.Domain.Interfaces.Configuration;
using Localizator.Auth.Domain.Interfaces.Strategy;
using Localizator.Shared.Result;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Localizator.Auth.Infrastructure.Strategies.Abstract;

public abstract class AuthStrategyBase<TOptions>(IAuthOptionsProvider provider) : IAuthStrategy where TOptions : IAuthOptions
{
    protected TOptions Options { get; } = (TOptions) provider.Get();
    public AuthMode Mode { get; init; } = provider.Get().Mode;
    public abstract Task<Result<bool>> AuthenticateAsync(HttpContext context, CancellationToken ct = default);

    public Result<bool> CheckIfUserLoggedIn(SignInManager<LocalizatorIdentityUser> signInManager, HttpContext context, string username)
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
}
