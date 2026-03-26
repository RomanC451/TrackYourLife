using System.ComponentModel;
using System.Text.Json.Serialization;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Youtube.Domain.Features.YoutubePlaylists;

[JsonConverter(typeof(StronglyTypedGuidJsonConverter<YoutubePlaylistId>))]
[TypeConverter(typeof(StronglyTypedGuidTypeConverter<YoutubePlaylistId>))]
public sealed record YoutubePlaylistId : StronglyTypedGuid<YoutubePlaylistId>
{
    public YoutubePlaylistId() { }

    public YoutubePlaylistId(Guid Value)
        : base(Value) { }

    public static bool TryParse(string? input, out YoutubePlaylistId? output)
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
