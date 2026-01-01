using System.ComponentModel;
using System.Text.Json.Serialization;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Youtube.Domain.Features.YoutubeSettings;

[JsonConverter(typeof(StronglyTypedGuidJsonConverter<YoutubeSettingsId>))]
[TypeConverter(typeof(StronglyTypedGuidTypeConverter<YoutubeSettingsId>))]
public sealed record YoutubeSettingsId : StronglyTypedGuid<YoutubeSettingsId>
{
    public YoutubeSettingsId() { }

    public YoutubeSettingsId(Guid Value)
        : base(Value) { }

    public static bool TryParse(string? input, out YoutubeSettingsId? output)
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
