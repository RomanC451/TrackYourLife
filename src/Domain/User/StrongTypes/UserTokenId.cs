using System.Text.Json.Serialization;
using TrackYourLifeDotnet.Domain.Shared;

namespace TrackYourLifeDotnet.Domain.Users.StrongTypes;

[JsonConverter(typeof(StrongGuidTypeJsonConverter<UserTokenId>))]
public record UserTokenId(Guid Value) : IStrongTypedGuid
{
    public static UserTokenId NewId() => new(Guid.NewGuid());

    public static UserTokenId Empty => new(Guid.Empty);
}
