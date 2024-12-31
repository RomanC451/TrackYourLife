using System.ComponentModel;
using System.Text.Json.Serialization;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;

[JsonConverter(typeof(StronglyTypedGuidJsonConverter<NutritionDiaryId>))]
[TypeConverter(typeof(StronglyTypedGuidTypeConverter<NutritionDiaryId>))]
public sealed record NutritionDiaryId : StronglyTypedGuid<NutritionDiaryId>
{
    public NutritionDiaryId() { }

    public NutritionDiaryId(Guid Value)
        : base(Value) { }

    public static bool TryParse(string? input, out NutritionDiaryId? output)
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
