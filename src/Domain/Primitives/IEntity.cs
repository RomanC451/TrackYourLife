namespace TrackYourLifeDotnet.Domain.Primitives;

public interface IEntity<Tid>
{
    Tid Id { get; }
}
