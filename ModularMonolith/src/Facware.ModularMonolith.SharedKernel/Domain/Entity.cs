namespace Facware.ModularMonolith.SharedKernel.Domain;

public abstract class Entity<TId> : IEquatable<Entity<TId>>
    where TId : notnull
{
    protected Entity(TId id)
    {
        Id = id;
    }

    public TId Id { get; protected set; }

    public override bool Equals(object? obj)
    {
        return obj is Entity<TId> other && Equals(other);
    }

    public bool Equals(Entity<TId>? other)
    {
        return other is not null && Id.Equals(other.Id);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public static bool operator ==(Entity<TId>? left, Entity<TId>? right)
    {
        return left is not null && right is not null && left.Equals(right);
    }

    public static bool operator !=(Entity<TId>? left, Entity<TId>? right)
    {
        return !(left == right);
    }
}