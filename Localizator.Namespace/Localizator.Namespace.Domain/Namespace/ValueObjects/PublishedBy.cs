using Localizator.Shared.Base;
using Localizator.Shared.Resources;

namespace Localizator.Namespace.Domain.Namespace.ValueObjects;

public sealed class PublishedBy(string value) : BaseValueObject<string>(Validate(value))
{
    private static string Validate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException(Errors.PublisherIdentifierCannotBeEmpty, nameof(value));

        return value.Trim();
    }
}
