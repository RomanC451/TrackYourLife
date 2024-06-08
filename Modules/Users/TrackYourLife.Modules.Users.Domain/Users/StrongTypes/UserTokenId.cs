using System.Text.Json.Serialization;
using TrackYourLife.Common.Domain.Shared;

namespace TrackYourLife.Modules.Users.Domain.Users.StrongTypes;

[JsonConverter(typeof(StronglyTypedGuidJsonConverter<UserTokenId>))]
public record UserTokenId(Guid Value) : IStronglyTypedGuid
{
    public static UserTokenId NewId() => new(Guid.NewGuid());

    public static UserTokenId Empty => new(Guid.Empty);
}
