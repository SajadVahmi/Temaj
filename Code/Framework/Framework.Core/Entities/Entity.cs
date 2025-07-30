namespace Framework.Core.Entities;

public abstract class Entity<TId> :
    IEntity<TId>
    where TId : notnull
{


    public TId Id { get;protected set; } = default!;

    #region Equality Check
    public bool Equals(Entity<TId>? other)
    {
        if (other is null)
            return false;

        return Id.Equals(other.Id);
    }

    public override bool Equals(object? obj) =>
        obj is Entity<TId> otherObject &&
        Id.Equals(otherObject.Id);

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(Entity<TId>? left, Entity<TId>? right)
    {
        if (left is null && right is null)
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    public static bool operator !=(Entity<TId> left, Entity<TId> right)
        => !(right == left);

    #endregion


    
}