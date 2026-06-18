using System.ComponentModel;
using System.Text.Json.Serialization;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;

[JsonConverter(typeof(StronglyTypedGuidJsonConverter<ReadingSessionNoteId>))]
[TypeConverter(typeof(StronglyTypedGuidTypeConverter<ReadingSessionNoteId>))]
public sealed record ReadingSessionNoteId : StronglyTypedGuid<ReadingSessionNoteId>
{
    public ReadingSessionNoteId() { }

    public ReadingSessionNoteId(Guid Value)
        : base(Value) { }

    public static bool TryParse(string? input, out ReadingSessionNoteId? output)
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
