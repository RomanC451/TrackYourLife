using System.Text.Json.Serialization;
using TrackYourLifeDotnet.Domain.Shared;

namespace TrackYourLifeDotnet.Domain.Foods.StrongTypes;

[JsonConverter(typeof(StrongGuidTypeJsonConverter<SearchedFoodId>))]
public sealed record SearchedFoodId(Guid Value) : IStrongTypedGuid
{
    public SearchedFoodId()
        : this(Guid.Empty) { }

    public static SearchedFoodId NewId() => new(Guid.NewGuid());

    public static SearchedFoodId Empty => new(Guid.Empty);
};
