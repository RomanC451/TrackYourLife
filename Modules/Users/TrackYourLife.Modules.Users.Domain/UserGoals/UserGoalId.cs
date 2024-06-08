using System.Text.Json.Serialization;
using TrackYourLife.Common.Domain.Shared;

namespace TrackYourLife.Modules.Users.Domain.UserGoals;

[JsonConverter(typeof(StronglyTypedGuidJsonConverter<UserGoalId>))]
public record UserGoalId(Guid Value) : IStronglyTypedGuid
{
    public UserGoalId()
        : this(Guid.Empty) { }

    public static UserGoalId NewId() => new(Guid.NewGuid());

    public static UserGoalId Empty => new(Guid.Empty);
}
