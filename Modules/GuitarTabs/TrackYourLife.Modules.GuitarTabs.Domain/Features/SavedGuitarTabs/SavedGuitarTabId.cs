using System.ComponentModel;
using System.Text.Json.Serialization;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.GuitarTabs.Domain.Features.SavedGuitarTabs;

[JsonConverter(typeof(StronglyTypedGuidJsonConverter<SavedGuitarTabId>))]
[TypeConverter(typeof(StronglyTypedGuidTypeConverter<SavedGuitarTabId>))]
public sealed record SavedGuitarTabId : StronglyTypedGuid<SavedGuitarTabId>
{
    public SavedGuitarTabId() { }

    public SavedGuitarTabId(Guid Value)
        : base(Value) { }

    public static bool TryParse(string? input, out SavedGuitarTabId? output)
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
