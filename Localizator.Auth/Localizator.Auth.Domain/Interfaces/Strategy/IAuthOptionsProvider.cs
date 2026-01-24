using Localizator.Auth.Domain.Interfaces.Configuration;

namespace Localizator.Auth.Domain.Interfaces.Strategy;

public interface IAuthOptionsProvider
{
    IAuthOptions Get();
}
