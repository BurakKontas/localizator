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

public sealed class NoneAuthStrategy(
    IAuthOptionsProvider provider,
    ILogger<NoneAuthStrategy> logger,
    UserManager<LocalizatorIdentityUser> userManager,
    SignInManager<LocalizatorIdentityUser> signInManager) : AuthStrategyBase<INoneAuthOptions>(provider)
{
    private readonly ILogger<NoneAuthStrategy> _logger = logger;
    private readonly UserManager<LocalizatorIdentityUser> _userManager = userManager;
    private readonly SignInManager<LocalizatorIdentityUser> _signInManager = signInManager;

    public override async Task<Result<int>> AuthenticateAsync(HttpContext context, CancellationToken ct = default)
    {
        var username = "devuser";

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
}
