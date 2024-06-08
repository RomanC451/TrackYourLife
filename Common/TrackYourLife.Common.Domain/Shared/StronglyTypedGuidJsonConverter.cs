using System.Text.Json;
using System.Text.Json.Serialization;

namespace TrackYourLife.Common.Domain.Shared;

public class StronglyTypedGuidJsonConverter<T> : JsonConverter<T>
    where T : IStronglyTypedGuid
{
    public override T? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        var guid = reader.GetGuid();
        var instance =
            Activator.CreateInstance(typeToConvert, guid)
            ?? throw new JsonException($"Unable to create an instance of type {typeToConvert}.");

        return (T)instance;
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value.ToString());
    }
}
