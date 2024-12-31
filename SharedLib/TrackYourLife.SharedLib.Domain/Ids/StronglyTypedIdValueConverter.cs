using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace TrackYourLife.SharedLib.Domain.Ids;

public class StronglyTypedIdValueConverter<T> : ValueConverter<T, Guid>
    where T : StronglyTypedGuid<T>, new()
{
    public StronglyTypedIdValueConverter()
        : base(id => id.Value, value => CreateInstance(value)) { }

    private static T CreateInstance(Guid value)
    {
        var instance = Activator.CreateInstance(typeof(T), value);
        if (instance == null)
        {
            throw new InvalidOperationException($"Unable to create instance of {typeof(T)}");
        }
        return (T)instance;
    }
}
