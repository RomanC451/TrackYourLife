namespace TrackYourLife.Common.Domain.Shared;

public class StronglyTypedGuid<T>
    where T : StronglyTypedGuid<T>, IStronglyTypedGuid, new()
{
    public Guid Value { get; private set; }

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

    public static T Empty()
    {
        T instance = new();
        instance.Value = Guid.Empty;
        return instance;
    }
}
