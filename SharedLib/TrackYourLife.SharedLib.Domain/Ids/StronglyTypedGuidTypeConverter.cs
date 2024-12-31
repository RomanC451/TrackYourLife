using System.ComponentModel;
using System.Globalization;

namespace TrackYourLife.SharedLib.Domain.Ids;

public class StronglyTypedGuidTypeConverter<T> : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
    }

    public override object ConvertFrom(
        ITypeDescriptorContext? context,
        CultureInfo? culture,
        object value
    )
    {
        var stringValue = value as string;

        if (string.IsNullOrEmpty(stringValue))
        {
            throw new ArgumentException("The value cannot be null or empty.", nameof(value));
        }

        if (!Guid.TryParse(stringValue, out var guid))
        {
            throw new FormatException($"The value '{stringValue}' is not a valid GUID.");
        }

        var instance =
            Activator.CreateInstance(typeof(T), guid)
            ?? throw new InvalidOperationException(
                $"Unable to create an instance of type {typeof(T)}."
            );

        return (T)instance;
    }
}
