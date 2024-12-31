using System.ComponentModel;
using System.Text.Json.Serialization;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;

[JsonConverter(typeof(StronglyTypedGuidJsonConverter<ServingSizeId>))]
[TypeConverter(typeof(StronglyTypedGuidTypeConverter<ServingSizeId>))]
public sealed record ServingSizeId : StronglyTypedGuid<ServingSizeId>
{
    public ServingSizeId() { }

    public ServingSizeId(Guid Value)
        : base(Value) { }

    public static bool TryParse(string? input, out ServingSizeId? output)
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
