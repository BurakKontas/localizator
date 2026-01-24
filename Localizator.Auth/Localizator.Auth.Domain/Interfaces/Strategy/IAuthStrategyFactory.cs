using Localizator.Auth.Domain.Interfaces.Configuration;

namespace Localizator.Auth.Domain.Interfaces.Strategy;

public interface IAuthStrategyFactory
{
    IAuthStrategy Create(IAuthOptions options);
}
