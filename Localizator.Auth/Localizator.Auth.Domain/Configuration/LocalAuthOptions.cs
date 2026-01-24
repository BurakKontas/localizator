using Localizator.Auth.Domain.Interfaces.Configuration;

namespace Localizator.Auth.Domain.Configuration;

public sealed class LocalAuthOptions : ILocalAuthOptions
{
    public AuthMode Mode => AuthMode.Local;

    public string AdminUser { get; init; } = default!;
    public string AdminPassword { get; init; } = default!;

    public override string ToString()
    {
        return $"Mode: {Mode}, AdminUser: {AdminUser}, AdminPassword: {"****"}";
    }
}
