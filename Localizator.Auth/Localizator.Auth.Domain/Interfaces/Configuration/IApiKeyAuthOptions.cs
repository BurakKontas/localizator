
namespace Localizator.Auth.Domain.Interfaces.Configuration;

public interface IApiKeyAuthOptions : IAuthOptions
{
    IReadOnlyDictionary<string, string> ApiKeys { get; }
}
