using System.ComponentModel;
using System.Text.Json.Serialization;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarTabDevices;

[JsonConverter(typeof(StronglyTypedGuidJsonConverter<GuitarTabDeviceId>))]
[TypeConverter(typeof(StronglyTypedGuidTypeConverter<GuitarTabDeviceId>))]
public sealed record GuitarTabDeviceId : StronglyTypedGuid<GuitarTabDeviceId>
{
    public GuitarTabDeviceId() { }

    public GuitarTabDeviceId(Guid Value)
        : base(Value) { }

    public static bool TryParse(string? input, out GuitarTabDeviceId? output)
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
