using Localizator.Shared.Base;
using Localizator.Shared.Resources;

namespace Localizator.Namespace.Domain.Namespace.ValueObjects;

public sealed class PublishedAt(DateTime value) : BaseValueObject<DateTime>(Validate(value))
{
    private static DateTime Validate(DateTime value)
    {
        if (value > DateTime.UtcNow)
            throw new ArgumentException(Errors.PublishedDateCannotBeFuture, nameof(value));

        return value;
    }

    public static PublishedAt Now() => new(DateTime.UtcNow);

    public bool IsOlderThan(TimeSpan duration)
    {
        return DateTime.UtcNow - Value > duration;
    }
}