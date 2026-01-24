using Localizator.Auth.Domain.Configuration;

namespace Localizator.Auth.Domain.Interfaces.Configuration;

public interface IAuthOptions
{
    AuthMode Mode { get; }
}
