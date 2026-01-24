using Localizator.Auth.Domain.Interfaces.Configuration;

namespace Localizator.Auth.Domain.Configuration;

public sealed class OidcAuthOptions : IOidcAuthOptions
{
    public AuthMode Mode => AuthMode.Oidc;

    public string Issuer { get; init; } = default!;
    public string ClientId { get; init; } = default!;
    public string ClientSecret { get; init; } = default!;
    public string RedirectUri { get; init; } = default!;

    public string? Scopes { get; init; }
    public string? Audience { get; init; }

    public override string ToString()
    {
        return $"Mode: {Mode}, Issuer: {Issuer}, ClientId: {ClientId}, ClientSecret: ****, RedirectUri: {RedirectUri}, Scopes: {Scopes}, Audience: {Audience}";
    }
}