using System.ComponentModel;
using System.Text.Json.Serialization;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Nutrition.Domain.Features.Foods;

[JsonConverter(typeof(StronglyTypedGuidJsonConverter<FoodId>))]
[TypeConverter(typeof(StronglyTypedGuidTypeConverter<FoodId>))]
public sealed record FoodId : StronglyTypedGuid<FoodId>
{
    public FoodId() { }

    public FoodId(Guid Value)
        : base(Value) { }

    public static bool TryParse(string? input, out FoodId? output)
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
};
