using System.Text.Json.Serialization;
using TrackYourLife.Common.Domain.Shared;

namespace TrackYourLife.Common.Domain.Foods;

[JsonConverter(typeof(StronglyTypedGuidJsonConverter<SearchedFoodId>))]
public sealed record SearchedFoodId(Guid Value) : IStronglyTypedGuid
{
    public SearchedFoodId()
        : this(Guid.Empty) { }

    public static SearchedFoodId NewId() => new(Guid.NewGuid());

    public static SearchedFoodId Empty => new(Guid.Empty);
};
