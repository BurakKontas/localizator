using Localizator.Shared.Base;
using Localizator.Shared.Extensions;
using Localizator.Shared.Resources;

namespace Localizator.Namespace.Domain.Namespace.ValueObjects;

public sealed class NamespaceStatus(string value) : BaseValueObject<string>(Validate(value))
{
    public static readonly NamespaceStatus Draft = new("draft");
    public static readonly NamespaceStatus Published = new("published");
    public static readonly NamespaceStatus Archived = new("archived");

    private static readonly HashSet<string> ValidStatuses = new() { "draft", "published", "archived" };

    private static string Validate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException(Errors.NamespaceStatusCannotBeEmpty, nameof(value));

        var lowerValue = value.ToLowerInvariant();
        if (!ValidStatuses.Contains(lowerValue))
            throw new ArgumentException(Errors.NamespaceStatusInvalid.Format(string.Join(", ", ValidStatuses)), nameof(value));

        return lowerValue;
    }

    public bool IsDraft => Value == "draft";
    public bool IsPublished => Value == "published";
    public bool IsArchived => Value == "archived";
}
