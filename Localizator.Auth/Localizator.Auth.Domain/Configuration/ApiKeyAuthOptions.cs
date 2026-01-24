using Localizator.Auth.Domain.Interfaces.Configuration;

namespace Localizator.Auth.Domain.Configuration;

public sealed class ApiKeyAuthOptions : IApiKeyAuthOptions
{
    public AuthMode Mode => AuthMode.ApiKey;
    public IReadOnlyDictionary<string, string> ApiKeys { get; init; } = new Dictionary<string, string>();

    public override string ToString()
    {
        var keys = ApiKeys.Count > 0
            ? string.Join(", ", ApiKeys.Select(kv => $"{kv.Key}:{kv.Value}"))
            : "None";

        return $"Mode: {Mode}, ApiKeys: {keys}";
    }
}
