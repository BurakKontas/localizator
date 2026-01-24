using Localizator.Auth.Domain.Configuration;
using Localizator.Auth.Domain.Interfaces.Configuration;
using Localizator.Auth.Domain.Interfaces.Strategy;
using Microsoft.AspNetCore.Http;

namespace Localizator.Auth.Infrastructure.Strategies.Abstract;

public abstract class AuthStrategyBase<TOptions>(IAuthOptionsProvider provider) : IAuthStrategy where TOptions : IAuthOptions
{
    protected TOptions Options { get; } = (TOptions) provider.Get();
    public abstract AuthMode Mode { get; }
    public abstract Task AuthenticateAsync(HttpContext context, CancellationToken ct = default);
}
