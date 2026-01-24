using Localizator.Auth.Domain.Interfaces.Configuration;

namespace Localizator.Auth.Domain.Interfaces.Strategy;

public interface IAuthOptionsFactory
{
    IAuthOptions Create();
}
