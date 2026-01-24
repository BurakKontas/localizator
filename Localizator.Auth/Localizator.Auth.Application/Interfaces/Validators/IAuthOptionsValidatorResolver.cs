using Localizator.Auth.Domain.Interfaces.Configuration;

namespace Localizator.Auth.Application.Interfaces.Validators;

public interface IAuthOptionsValidatorResolver
{
    void Validate(IAuthOptions options);
}
