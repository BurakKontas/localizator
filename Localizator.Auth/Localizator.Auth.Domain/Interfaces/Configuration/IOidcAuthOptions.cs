namespace Localizator.Auth.Domain.Interfaces.Configuration;

public interface IOidcAuthOptions : IAuthOptions
{
    string Issuer { get; }
    string ClientId { get; }
    string ClientSecret { get; }
    string RedirectUri { get; }

    string? Scopes { get; }
    string? Audience { get; }
}

