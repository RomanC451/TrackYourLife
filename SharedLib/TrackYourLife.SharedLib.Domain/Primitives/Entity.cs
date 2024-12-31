using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.SharedLib.Domain.Primitives;

public abstract class Entity<TId> : IEquatable<Entity<TId>>, IEntity<TId>
    where TId : IStronglyTypedGuid
{
    protected Entity(TId id) => Id = id;

    protected Entity() { }

    public virtual TId Id { get; set; } = default!;

    public static bool operator ==(Entity<TId>? first, Entity<TId>? second) =>
        first is not null && second is not null && first.Equals(second);

    public static bool operator !=(Entity<TId>? first, Entity<TId>? second) => !(first == second);

    public bool Equals(Entity<TId>? other)
    {
        if (other is null)
        {
            return false;
        }

        if (other.GetType() != GetType())
        {
            return false;
        }

        return EqualityComparer<TId>.Default.Equals(other.Id, Id);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (obj.GetType() != GetType())
        {
            return false;
        }

        if (obj is not Entity<TId> entity)
        {
            return false;
        }

        return EqualityComparer<TId>.Default.Equals(entity.Id, Id);
    }

    public override int GetHashCode() => Id?.GetHashCode() ?? 0 * 41;

}
