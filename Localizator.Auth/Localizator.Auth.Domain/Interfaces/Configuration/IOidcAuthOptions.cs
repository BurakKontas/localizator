namespace Localizator.Auth.Domain.Interfaces.Configuration;

public interface IOidcAuthOptions : IAuthOptions
{
    string Issuer { get; }
    string Audience { get; }
    string ConfigurationUrl { get; } 
}

