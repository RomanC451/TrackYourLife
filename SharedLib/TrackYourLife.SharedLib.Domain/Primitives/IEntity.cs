namespace TrackYourLife.SharedLib.Domain.Primitives;

public interface IEntity<out Tid>
{
    Tid Id { get; }
}
