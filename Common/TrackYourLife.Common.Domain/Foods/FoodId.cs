using System.Text.Json.Serialization;
using TrackYourLife.Common.Domain.Shared;

namespace TrackYourLife.Common.Domain.Foods;

[JsonConverter(typeof(StronglyTypedGuidJsonConverter<FoodId>))]
public record FoodId(Guid Value) : IStronglyTypedGuid
{
    public static FoodId NewId() => new(Guid.NewGuid());

    public static FoodId Empty => new(Guid.Empty);
}
