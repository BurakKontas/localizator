using Localizator.Shared.Base;
using Localizator.Shared.Resources;

namespace Localizator.Namespace.Domain.Namespace.ValueObjects;

public sealed class NamespaceSlug(string value) : BaseValueObject<string>(Validate(value))
{
    public static readonly int MAX_LENGTH = 20;

    private static string Validate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException(Errors.NamespaceSlugCannotBeEmpty, nameof(value));

        if (!System.Text.RegularExpressions.Regex.IsMatch(value, "^[a-z0-9-]+$"))
            throw new ArgumentException(Errors.NamespaceSlugInvalidFormat, nameof(value));

        if (value.Length > MAX_LENGTH)
            throw new ArgumentException(Errors.NamespaceSlugTooLong, nameof(value));

        return value;
    }
}
