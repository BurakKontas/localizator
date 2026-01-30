using Localizator.Shared.Base;
using Localizator.Shared.Resources;

namespace Localizator.Namespace.Domain.Namespace.ValueObjects;

public sealed class NamespaceName(string value) : BaseValueObject<string>(Validate(value))
{
    private static string Validate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException(Errors.NamespaceNameCannotBeEmpty, nameof(value));

        if (value.Length > 100)
            throw new ArgumentException(Errors.NamespaceNameTooLong, nameof(value));

        return value.Trim();
    }
}
