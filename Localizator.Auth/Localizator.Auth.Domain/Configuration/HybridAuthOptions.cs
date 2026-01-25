using Localizator.Auth.Domain.Configuration.Mode;
using Localizator.Auth.Domain.Interfaces.Configuration;

namespace Localizator.Auth.Domain.Configuration;

public sealed class HybridAuthOptions : IHybridAuthOptions
{
    public AuthMode Mode => AuthMode.Hybrid;

    // === OIDC ===
    public string Issuer { get; init; } = default!;
    public string Audience { get; init; } = default!;
    public string ConfigurationUrl { get; init; } = default!;

    // === API KEY ===
    public IReadOnlyDictionary<string, string> ApiKeys { get; init; } = new Dictionary<string, string>();


    public override string ToString()
    {
        var apiKeys = ApiKeys.Count > 0
            ? string.Join(", ", ApiKeys.Select(kv => $"{kv.Key}:{kv.Value}"))
            : "None";
        
        return $"Mode: {Mode}, Issuer: {Issuer}, Audience: {Audience}, ApiKeys: {apiKeys}";
    }
}