using System.ComponentModel;
using System.Text.Json.Serialization;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Nutrition.Domain.Features.FoodsHistory;

[JsonConverter(typeof(StronglyTypedGuidJsonConverter<FoodHistoryId>))]
[TypeConverter(typeof(StronglyTypedGuidTypeConverter<FoodHistoryId>))]
public sealed record FoodHistoryId : StronglyTypedGuid<FoodHistoryId>
{
    public FoodHistoryId() { }

    public FoodHistoryId(Guid value)
        : base(value) { }
}
