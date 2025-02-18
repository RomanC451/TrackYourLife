using System.ComponentModel;
using System.Text.Json.Serialization;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Nutrition.Domain.Features.DailyNutritionOverviews;

[JsonConverter(typeof(StronglyTypedGuidJsonConverter<DailyNutritionOverviewId>))]
[TypeConverter(typeof(StronglyTypedGuidTypeConverter<DailyNutritionOverviewId>))]
public sealed record DailyNutritionOverviewId : StronglyTypedGuid<DailyNutritionOverviewId>
{
    public DailyNutritionOverviewId() { }

    public DailyNutritionOverviewId(Guid Value)
        : base(Value) { }

    public static bool TryParse(string? input, out DailyNutritionOverviewId? output)
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
