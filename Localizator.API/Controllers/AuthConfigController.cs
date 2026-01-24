using Localizator.Auth.Domain.Interfaces.Configuration;
using Localizator.Auth.Domain.Interfaces.Strategy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi;

namespace Localizator.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthConfigController(IAuthOptions options, IAuthStrategy authStrategy) : ControllerBase
{
    private readonly IAuthOptions _options = options;
    private readonly IAuthStrategy _authStrategy = authStrategy;

    [HttpGet("config")]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        await _authStrategy.AuthenticateAsync(HttpContext, cancellationToken);
        return Ok(new Dictionary<string, string>()
        {
            { "_options.mode", _options.Mode.GetDisplayName() },
            { "_authStrategy.mode", _authStrategy.Mode.GetDisplayName() },
        });
    }
}
