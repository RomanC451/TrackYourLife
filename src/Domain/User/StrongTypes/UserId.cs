using System.Text.Json.Serialization;
using TrackYourLifeDotnet.Domain.Shared;

namespace TrackYourLifeDotnet.Domain.Users.StrongTypes;

[JsonConverter(typeof(StrongGuidTypeJsonConverter<UserId>))]
public record UserId(Guid Value) : IStrongTypedGuid
{
    public UserId()
        : this(Guid.Empty) { }

    public static UserId NewId() => new(Guid.NewGuid());

    public static UserId Empty => new(Guid.Empty);
}
