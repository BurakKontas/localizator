using Localizator.Shared.Base;
using Localizator.Shared.Resources;
using System.ComponentModel;
using System.Security;

namespace Localizator.Namespace.Domain.Namespace.ValueObjects;

public sealed class NamespacePermission(string value) : BaseValueObject<string>(Validate(value))
{
    public static readonly string CREATOR = "creator";
    public static readonly string ADMIN = "admin";
    public static readonly string READ = "read";
    public static readonly string PUBLISHER = "publisher";
    public static readonly string WRITE = "write";

    public string Permission { get; set; } = value;

    public static readonly List<string> PERMISSIONS = [CREATOR, ADMIN, READ, WRITE, PUBLISHER];

    private static string Validate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException(Errors.NamespacePermissionCannotBeNull, nameof(value));

        if (!PERMISSIONS.Contains(value.ToUpper()))
            throw new ArgumentException(Errors.InvalidNamespacePermission, nameof(value));

        return value.ToUpper();
    }

    public static bool HasPermission(NamespacePermission current, NamespacePermission required)
    {
        if (current is null || required is null)
            return false;

        return GetLevel(current) >= GetLevel(required);
    }

    public bool HasPermission(NamespacePermission required)
    {
        return HasPermission(this, required);
    }

    public bool HasPermission(string required)
    {
        return HasPermission(this, new NamespacePermission(required));
    }

    private static int GetLevel(NamespacePermission permission)
    {
        return permission.Value switch
        {
            var v when v.Equals(CREATOR, StringComparison.CurrentCultureIgnoreCase) => 5,
            var v when v.Equals(ADMIN, StringComparison.CurrentCultureIgnoreCase) => 4,
            var v when v.Equals(WRITE, StringComparison.CurrentCultureIgnoreCase) => 3,
            var v when v.Equals(PUBLISHER, StringComparison.CurrentCultureIgnoreCase) => 2,
            var v when v.Equals(READ, StringComparison.CurrentCultureIgnoreCase) => 1,
            _ => 0
        };
    }
}
