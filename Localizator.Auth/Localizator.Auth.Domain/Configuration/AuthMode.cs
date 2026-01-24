namespace Localizator.Auth.Domain.Configuration;

public enum AuthMode
{
    Oidc,
    Local,
    Header,
    ApiKey,
    Hybrid
}