namespace Localizator.Auth.Domain.Interfaces.Configuration;

public interface IHeaderAuthOptions : IAuthOptions
{
    string UserHeader { get; }
    string? EmailHeader { get; }
}
