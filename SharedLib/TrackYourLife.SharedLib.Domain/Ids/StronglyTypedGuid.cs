using System.Runtime.Serialization;

namespace TrackYourLife.SharedLib.Domain.Ids;

[KnownType(typeof(Guid))]
public record StronglyTypedGuid<T> : IStronglyTypedGuid
    where T : StronglyTypedGuid<T>, new()
{
    public Guid Value { get; private set; }

    public StronglyTypedGuid() { }

    public StronglyTypedGuid(Guid Value)
    {
        this.Value = Value;
    }

    public static T Create(Guid guid)
    {
        T instance = new();
        instance.Value = guid;
        return instance;
    }

    public static T NewId()
    {
        T instance = new();
        instance.Value = Guid.NewGuid();
        return instance;
    }

    public static T Empty => Create(Guid.Empty);
}
