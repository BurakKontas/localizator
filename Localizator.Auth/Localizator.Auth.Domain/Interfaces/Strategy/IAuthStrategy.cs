using Localizator.Auth.Domain.Configuration.Mode;
using Localizator.Shared.Result;
using Microsoft.AspNetCore.Http;

namespace Localizator.Auth.Domain.Interfaces.Strategy;

public interface IAuthStrategy
{
    AuthMode Mode { get; }
    Task<Result<int>> AuthenticateAsync(HttpContext context, CancellationToken ct = default);
}
