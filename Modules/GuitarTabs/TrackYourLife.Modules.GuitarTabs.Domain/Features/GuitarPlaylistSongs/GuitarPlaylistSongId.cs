using System.ComponentModel;
using System.Text.Json.Serialization;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarPlaylistSongs;

[JsonConverter(typeof(StronglyTypedGuidJsonConverter<GuitarPlaylistSongId>))]
[TypeConverter(typeof(StronglyTypedGuidTypeConverter<GuitarPlaylistSongId>))]
public sealed record GuitarPlaylistSongId : StronglyTypedGuid<GuitarPlaylistSongId>
{
    public GuitarPlaylistSongId() { }

    public GuitarPlaylistSongId(Guid Value)
        : base(Value) { }

    public override string ToString() => Value.ToString();
}
