namespace Facware.ModularMonolith.SharedKernel.Domain;

public abstract class ValueObject : IEquatable<ValueObject>
{
    protected abstract IEnumerable<object?> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        return obj is ValueObject other && Equals(other);
    }

    public bool Equals(ValueObject? other)
    {
        return other is not null && GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(component => component?.GetHashCode() ?? 0)
            .Aggregate((current, next) => HashCode.Combine(current, next));
    }

    public static bool operator ==(ValueObject? left, ValueObject? right)
    {
        return left is not null && right is not null && left.Equals(right);
    }

    public static bool operator !=(ValueObject? left, ValueObject? right)
    {
        return !(left == right);
    }
}