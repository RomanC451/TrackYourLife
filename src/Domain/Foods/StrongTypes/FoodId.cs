using System.Text.Json.Serialization;
using TrackYourLifeDotnet.Domain.Shared;

namespace TrackYourLifeDotnet.Domain.Foods.StrongTypes;

[JsonConverter(typeof(StrongGuidTypeJsonConverter<FoodId>))]
public record FoodId(Guid Value) : IStrongTypedGuid
{
    public static FoodId NewId() => new(Guid.NewGuid());

    public static FoodId Empty => new(Guid.Empty);
}
