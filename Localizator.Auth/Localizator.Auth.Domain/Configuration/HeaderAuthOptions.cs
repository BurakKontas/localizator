using Localizator.Auth.Domain.Interfaces.Configuration;

namespace Localizator.Auth.Domain.Configuration;

public sealed class HeaderAuthOptions : IHeaderAuthOptions
{
    public AuthMode Mode => AuthMode.Header;

    public string UserHeader { get; init; } = default!;
    public string? EmailHeader { get; init; }

    public override string ToString()
    {
        return $"Mode: {Mode}, UserHeader: {UserHeader}, EmailHeader: {EmailHeader}";
    }
}
