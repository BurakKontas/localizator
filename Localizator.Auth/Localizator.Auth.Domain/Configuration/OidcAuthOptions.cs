using Localizator.Auth.Domain.Configuration.Mode;
using Localizator.Auth.Domain.Interfaces.Configuration;

namespace Localizator.Auth.Domain.Configuration;

public sealed class OidcAuthOptions : IOidcAuthOptions
{
    public AuthMode Mode => AuthMode.Oidc;

    public string Issuer { get; init; } = default!;
    public string Audience { get; init; } = default!;
    public string ConfigurationUrl { get; init; } = default!;

    public override string ToString()
    {
        return $"Mode: {Mode}, Issuer: {Issuer}, Audience: {Audience}";
    }
}