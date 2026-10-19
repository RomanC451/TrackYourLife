using System.ComponentModel;
using System.Text.Json.Serialization;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarPlaylists;

[JsonConverter(typeof(StronglyTypedGuidJsonConverter<GuitarPlaylistId>))]
[TypeConverter(typeof(StronglyTypedGuidTypeConverter<GuitarPlaylistId>))]
public sealed record GuitarPlaylistId : StronglyTypedGuid<GuitarPlaylistId>
{
    public GuitarPlaylistId() { }

    public GuitarPlaylistId(Guid Value)
        : base(Value) { }

    public static bool TryParse(string? input, out GuitarPlaylistId? output)
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
