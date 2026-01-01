using System.ComponentModel;
using System.Text.Json.Serialization;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Youtube.Domain.Features.WatchedVideos;

[JsonConverter(typeof(StronglyTypedGuidJsonConverter<WatchedVideoId>))]
[TypeConverter(typeof(StronglyTypedGuidTypeConverter<WatchedVideoId>))]
public sealed record WatchedVideoId : StronglyTypedGuid<WatchedVideoId>
{
    public WatchedVideoId() { }

    public WatchedVideoId(Guid Value)
        : base(Value) { }

    public static bool TryParse(string? input, out WatchedVideoId? output)
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
