using Localizator.Namespace.Domain.Namespace.Enums;
using Localizator.Shared.Base;
using Localizator.Shared.Resources;

namespace Localizator.Namespace.Domain.Namespace.ValueObjects;

public sealed class NamespaceVersion(string value) : BaseValueObject<string>(Validate(value))
{
    private static string Validate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException(Errors.NamespaceVersionCannotBeEmpty, nameof(value));

        // Semantic versioning pattern: major.minor.patch
        if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^\d+\.\d+\.\d+$"))
            throw new ArgumentException(Errors.NamespaceVersionInvalidFormat, nameof(value));

        return value;
    }
    public static NamespaceVersion Initial() => new("1.0.0");

    public NamespaceVersion Bump(VersionBumpType bumpType)
    {
        var parts = Value.Split('.');
        var major = int.Parse(parts[0]);
        var minor = int.Parse(parts[1]);
        var patch = int.Parse(parts[2]);

        return bumpType switch
        {
            VersionBumpType.Major => new NamespaceVersion($"{major + 1}.0.0"),
            VersionBumpType.Minor => new NamespaceVersion($"{major}.{minor + 1}.0"),
            VersionBumpType.Patch => new NamespaceVersion($"{major}.{minor}.{patch + 1}"),
            _ => throw new ArgumentException(Errors.InvalidVersionBumpType)
        };
    }

    public int GetMajorVersion()
    {
        return int.Parse(Value.Split('.')[0]);
    }

    public int GetMinorVersion()
    {
        return int.Parse(Value.Split('.')[1]);
    }

    public int GetPatchVersion()
    {
        return int.Parse(Value.Split('.')[2]);
    }
}
