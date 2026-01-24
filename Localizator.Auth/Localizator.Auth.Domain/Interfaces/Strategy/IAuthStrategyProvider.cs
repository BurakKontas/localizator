namespace Localizator.Auth.Domain.Interfaces.Strategy;

public interface IAuthStrategyProvider
{
    IAuthStrategy Get();
}
