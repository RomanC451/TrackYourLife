using System.ComponentModel;
using System.Text.Json.Serialization;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;

[JsonConverter(typeof(StronglyTypedGuidJsonConverter<ReadingSessionId>))]
[TypeConverter(typeof(StronglyTypedGuidTypeConverter<ReadingSessionId>))]
public sealed record ReadingSessionId : StronglyTypedGuid<ReadingSessionId>
{
    public ReadingSessionId() { }

    public ReadingSessionId(Guid Value)
        : base(Value) { }

    public static bool TryParse(string? input, out ReadingSessionId? output)
    {
        output = null;

        if (string.IsNullOrEmpty(input))
        {
            return false;
        }

        if (!Guid.TryParse(input, out var guid))
        {
            return false;
        }

        output = Create(guid);

        return true;
    }

    public override string ToString() => Value.ToString();
}
