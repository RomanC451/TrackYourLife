namespace TrackYourLife.Common.Domain.Primitives;

public interface IEntity<Tid>
{
    Tid Id { get; }
}
