using System.Text.Json.Serialization;
using TrackYourLifeDotnet.Domain.Shared;

namespace TrackYourLifeDotnet.Domain.Foods.StrongTypes;

[JsonConverter(typeof(StrongGuidTypeJsonConverter<ServingSizeId>))]
public record ServingSizeId(Guid Value) : IStrongTypedGuid
{
    public ServingSizeId()
        : this(Guid.Empty) { }

    public static ServingSizeId NewId() => new(Guid.NewGuid());

    public static ServingSizeId Empty => new(Guid.Empty);
}
