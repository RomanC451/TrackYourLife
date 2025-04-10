using System.ComponentModel;
using System.Text.Json.Serialization;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Users.Domain.Features.Tokens;

[JsonConverter(typeof(StronglyTypedGuidJsonConverter<DeviceId>))]
[TypeConverter(typeof(StronglyTypedGuidTypeConverter<DeviceId>))]
public sealed record DeviceId : StronglyTypedGuid<DeviceId>
{
    public DeviceId() { }

    public DeviceId(Guid Value)
        : base(Value) { }

    public static bool TryParse(string? input, out DeviceId? output)
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
