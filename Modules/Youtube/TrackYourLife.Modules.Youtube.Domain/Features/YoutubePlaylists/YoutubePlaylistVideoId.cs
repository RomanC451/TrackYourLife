using System.ComponentModel;
using System.Text.Json.Serialization;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Youtube.Domain.Features.YoutubePlaylists;

[JsonConverter(typeof(StronglyTypedGuidJsonConverter<YoutubePlaylistVideoId>))]
[TypeConverter(typeof(StronglyTypedGuidTypeConverter<YoutubePlaylistVideoId>))]
public sealed record YoutubePlaylistVideoId : StronglyTypedGuid<YoutubePlaylistVideoId>
{
    public YoutubePlaylistVideoId() { }

    public YoutubePlaylistVideoId(Guid Value)
        : base(Value) { }

    public static bool TryParse(string? input, out YoutubePlaylistVideoId? output)
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
