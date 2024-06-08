using System.Text.Json.Serialization;
using TrackYourLife.Common.Domain.Shared;

namespace TrackYourLife.Common.Domain.ServingSizes;

[JsonConverter(typeof(StronglyTypedGuidJsonConverter<ServingSizeId>))]
public record ServingSizeId(Guid Value) : IStronglyTypedGuid
{
    public ServingSizeId()
        : this(Guid.Empty) { }

    public static ServingSizeId NewId() => new(Guid.NewGuid());

    public static ServingSizeId Empty => new(Guid.Empty);
}
