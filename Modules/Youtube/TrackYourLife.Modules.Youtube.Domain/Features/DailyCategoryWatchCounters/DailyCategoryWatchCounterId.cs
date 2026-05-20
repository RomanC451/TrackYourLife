using System.ComponentModel;
using System.Text.Json.Serialization;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Youtube.Domain.Features.DailyCategoryWatchCounters;

[JsonConverter(typeof(StronglyTypedGuidJsonConverter<DailyCategoryWatchCounterId>))]
[TypeConverter(typeof(StronglyTypedGuidTypeConverter<DailyCategoryWatchCounterId>))]
public sealed record DailyCategoryWatchCounterId : StronglyTypedGuid<DailyCategoryWatchCounterId>
{
    public DailyCategoryWatchCounterId() { }

    public DailyCategoryWatchCounterId(Guid Value)
        : base(Value) { }

    public static bool TryParse(string? input, out DailyCategoryWatchCounterId? output)
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

    public override string ToString()
    {
        return Value.ToString();
    }
}
