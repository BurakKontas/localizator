using Localizator.Auth.Domain.Configuration;
using Microsoft.AspNetCore.Http;

namespace Localizator.Auth.Domain.Interfaces.Strategy;

public interface IAuthStrategy
{
    AuthMode Mode { get; }
    Task AuthenticateAsync(HttpContext context, CancellationToken ct = default);
}
