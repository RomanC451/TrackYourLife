using System.Text.Json.Serialization;
using TrackYourLife.Common.Domain.Shared;

namespace TrackYourLife.Common.Domain.Users;

[JsonConverter(typeof(StronglyTypedGuidJsonConverter<UserId>))]
public record UserId(Guid Value) : IStronglyTypedGuid
{
    public UserId()
        : this(Guid.Empty) { }

    public static UserId NewId() => new(Guid.NewGuid());

    public static UserId Empty => new(Guid.Empty);
}
