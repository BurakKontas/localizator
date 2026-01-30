using System.Numerics;

namespace Localizator.Shared.Base;

public abstract class BaseNumericValueObject<T>(T value) where T : IComparable<T>, INumber<T>
{
    public T Value { get; init; } = value;

    public override string? ToString()
    {
        return Value.ToString();
    }

    public static implicit operator T(BaseNumericValueObject<T> vo)
    {
        return vo.Value;
    }

    public static bool operator >(BaseNumericValueObject<T> left, BaseNumericValueObject<T> right)
    {
        return left.Value.CompareTo(right.Value) > 0;
    }

    public static bool operator >=(BaseNumericValueObject<T> left, BaseNumericValueObject<T> right)
    {
        return left.Value.CompareTo(right.Value) >= 0;
    }

    public static bool operator <(BaseNumericValueObject<T> left, BaseNumericValueObject<T> right)
    {
        return left.Value.CompareTo(right.Value) < 0;
    }

    public static bool operator <=(BaseNumericValueObject<T> left, BaseNumericValueObject<T> right)
    {
        return left.Value.CompareTo(right.Value) <= 0;
    }

    public static bool operator ==(BaseNumericValueObject<T>? left, BaseNumericValueObject<T>? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;
        return left.Value.CompareTo(right.Value) == 0;
    }

    public static bool operator !=(BaseNumericValueObject<T>? left, BaseNumericValueObject<T>? right)
    {
        return !(left == right);
    }

    // Arithmetic operators
    public static T operator +(BaseNumericValueObject<T> left, BaseNumericValueObject<T> right)
    {
        return left.Value + right.Value;
    }

    public static T operator -(BaseNumericValueObject<T> left, BaseNumericValueObject<T> right)
    {
        return left.Value - right.Value;
    }

    public static T operator *(BaseNumericValueObject<T> left, BaseNumericValueObject<T> right)
    {
        return left.Value * right.Value;
    }

    public static T operator /(BaseNumericValueObject<T> left, BaseNumericValueObject<T> right)
    {
        return left.Value / right.Value;
    }

    public static T operator %(BaseNumericValueObject<T> left, BaseNumericValueObject<T> right)
    {
        return left.Value % right.Value;
    }

    // Unary operators
    public static T operator +(BaseNumericValueObject<T> vo)
    {
        return +vo.Value;
    }

    public static T operator -(BaseNumericValueObject<T> vo)
    {
        return -vo.Value;
    }

    // Equality
    public override bool Equals(object? obj)
    {
        if (obj is BaseNumericValueObject<T> other)
            return Value.CompareTo(other.Value) == 0;
        return false;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}