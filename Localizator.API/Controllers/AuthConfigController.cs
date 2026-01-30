using Localizator.Auth.Application.LocalizatorAuthorize;
using Localizator.Auth.Domain.Interfaces.Configuration;
using Localizator.Auth.Domain.Interfaces.Strategy;
using Localizator.Shared.Mediator.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi;

namespace Localizator.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthConfigController(IAuthOptions options, IAuthStrategy authStrategy, IMediator mediator) : ControllerBase
{
    private readonly IAuthOptions _options = options;
    private readonly IAuthStrategy _authStrategy = authStrategy;
    private readonly IMediator _mediator = mediator;

    [HttpGet("config")]
    [LocalizatorAuthorize]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        throw new TimeoutException();

        await _authStrategy.AuthenticateAsync(HttpContext, cancellationToken);
        return Ok(new Dictionary<string, string>()
        {
            { "_options.mode", _options.Mode.GetDisplayName() },
            { "_authStrategy.mode", _authStrategy.Mode.GetDisplayName() },
            { "_user.name", User.Identity?.Name ?? "anonymous" }
        });
    }
}
