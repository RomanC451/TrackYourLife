using System.ComponentModel;
using System.Text.Json.Serialization;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Nutrition.Domain.Features.SearchedFoods;

[JsonConverter(typeof(StronglyTypedGuidJsonConverter<SearchedFoodId>))]
[TypeConverter(typeof(StronglyTypedGuidTypeConverter<SearchedFoodId>))]
public sealed record SearchedFoodId : StronglyTypedGuid<SearchedFoodId>
{
    public SearchedFoodId() { }

    public SearchedFoodId(Guid Value)
        : base(Value) { }

    public static bool TryParse(string? input, out SearchedFoodId? output)
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
