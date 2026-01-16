using System.ComponentModel;
using System.Text.Json.Serialization;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;

[JsonConverter(typeof(StronglyTypedGuidJsonConverter<YoutubeChannelId>))]
[TypeConverter(typeof(StronglyTypedGuidTypeConverter<YoutubeChannelId>))]
public sealed record YoutubeChannelId : StronglyTypedGuid<YoutubeChannelId>
{
    public YoutubeChannelId() { }

    public YoutubeChannelId(Guid Value)
        : base(Value) { }

    public static bool TryParse(string? input, out YoutubeChannelId? output)
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
