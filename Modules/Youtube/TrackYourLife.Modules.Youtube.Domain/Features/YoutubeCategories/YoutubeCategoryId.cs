using System.ComponentModel;
using System.Text.Json.Serialization;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;

[JsonConverter(typeof(StronglyTypedGuidJsonConverter<YoutubeCategoryId>))]
[TypeConverter(typeof(StronglyTypedGuidTypeConverter<YoutubeCategoryId>))]
public sealed record YoutubeCategoryId : StronglyTypedGuid<YoutubeCategoryId>
{
    public YoutubeCategoryId() { }

    public YoutubeCategoryId(Guid Value)
        : base(Value) { }

    public static bool TryParse(string? input, out YoutubeCategoryId? output)
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
