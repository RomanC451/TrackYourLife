using System.ComponentModel.DataAnnotations.Schema;
using TrackYourLifeDotnet.Domain.Primitives;
using TrackYourLifeDotnet.Domain.Enums;

namespace TrackYourLifeDotnet.Domain.Entities;

public class UserToken : Entity
{
    public UserToken(Guid id, string value, Guid userId, UserTokenTypes type)
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
    public Guid UserId { get; init; }

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
