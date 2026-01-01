using System.ComponentModel;
using System.Text.Json.Serialization;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Youtube.Domain.Features.DailyDivertissmentCounters;

[JsonConverter(typeof(StronglyTypedGuidJsonConverter<DailyDivertissmentCounterId>))]
[TypeConverter(typeof(StronglyTypedGuidTypeConverter<DailyDivertissmentCounterId>))]
public sealed record DailyDivertissmentCounterId : StronglyTypedGuid<DailyDivertissmentCounterId>
{
    public DailyDivertissmentCounterId() { }

    public DailyDivertissmentCounterId(Guid Value)
        : base(Value) { }

    public static bool TryParse(string? input, out DailyDivertissmentCounterId? output)
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
