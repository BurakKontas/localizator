namespace Localizator.Shared.Base;

public class BaseValueObject<T>(T value)
{
    public BaseValueObject() : this(default!) { } // EF

    public T Value { get; init; } = value ?? throw new ArgumentNullException(nameof(value));

    public static T Empty { get; } = default!;

    public override string? ToString()
    {
        return Value?.ToString();
    }

    public static implicit operator string?(BaseValueObject<T> vo)
    {
        return vo.Value?.ToString();
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj)) return true;
        if (obj is null || obj.GetType() != GetType()) return false;

        var other = (BaseValueObject<T>)obj!;
        return EqualityComparer<T>.Default.Equals(Value, other.Value);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Value);
    }
}
