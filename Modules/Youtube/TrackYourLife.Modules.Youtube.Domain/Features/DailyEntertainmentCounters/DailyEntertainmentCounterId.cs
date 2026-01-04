using System.ComponentModel;
using System.Text.Json.Serialization;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Youtube.Domain.Features.DailyEntertainmentCounters;

[JsonConverter(typeof(StronglyTypedGuidJsonConverter<DailyEntertainmentCounterId>))]
[TypeConverter(typeof(StronglyTypedGuidTypeConverter<DailyEntertainmentCounterId>))]
public sealed record DailyEntertainmentCounterId : StronglyTypedGuid<DailyEntertainmentCounterId>
{
    public DailyEntertainmentCounterId() { }

    public DailyEntertainmentCounterId(Guid Value)
        : base(Value) { }

    public static bool TryParse(string? input, out DailyEntertainmentCounterId? output)
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
