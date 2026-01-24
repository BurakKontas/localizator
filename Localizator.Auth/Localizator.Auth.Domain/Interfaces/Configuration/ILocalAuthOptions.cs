namespace Localizator.Auth.Domain.Interfaces.Configuration;

public interface ILocalAuthOptions : IAuthOptions
{
    string AdminUser { get; }
    string AdminPassword { get; }
}
