using System.Text.Json.Serialization;
using TrackYourLifeDotnet.Domain.Shared;

namespace TrackYourLifeDotnet.Domain.FoodDiaries;

[JsonConverter(typeof(StrongGuidTypeJsonConverter<FoodDiaryEntryId>))]
public record FoodDiaryEntryId(Guid Value) : IStrongTypedGuid
{
    public FoodDiaryEntryId()
        : this(Guid.Empty) { }

    public static FoodDiaryEntryId NewId() => new(Guid.NewGuid());

    public static FoodDiaryEntryId Empty => new(Guid.Empty);
}
