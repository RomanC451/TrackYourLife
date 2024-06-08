using System.Text.Json;
using System.Text.Json.Serialization;

namespace TrackYourLife.Common.Application.Core.Converters;

public class DateOnlyConverter : JsonConverter<DateOnly>
{
    public override DateOnly Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        var dateString = reader.GetString();
        if (dateString == null)
        {
            throw new JsonException("Date string is null.");
        }
        return DateOnly.Parse(dateString);
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
