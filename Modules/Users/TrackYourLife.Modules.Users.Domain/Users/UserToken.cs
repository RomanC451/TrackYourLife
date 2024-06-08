
using TrackYourLife.Common.Domain.Primitives;
using TrackYourLife.Common.Domain.Shared;
using TrackYourLife.Modules.Users.Domain.Users.StrongTypes;

namespace TrackYourLife.Modules.Users.Domain.Users;

public class UserToken : Entity<UserTokenId>
{
    public UserToken(UserTokenId id, string value, UserId userId, UserTokenTypes type)
        : base(id)
    {
        Value = value;
        UserId = userId;
        CreatedOn = DateTime.UtcNow;
        Type = type;
        ExpiresAt = DateTime.UtcNow.AddMinutes(
            UserTokensMinutesToExpires.GetMinutesToExpire(Type.ToString())
        );
    }

    public UserTokenTypes Type { get; init; }

    public string Value { get; private set; }

    public DateTime CreatedOn { get; init; }

    public DateTime ExpiresAt { get; private set; }
    public UserId UserId { get; init; }

    public void UpdateToken(string newValue)
    {
        Value = newValue;
        RefreshLifeTime();
    }

    public void RefreshLifeTime()
    {
        ExpiresAt = DateTime.UtcNow.AddMinutes(
            UserTokensMinutesToExpires.GetMinutesToExpire(Type.ToString())
        );
    }
}
