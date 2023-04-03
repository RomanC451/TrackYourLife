using System.ComponentModel.DataAnnotations.Schema;
using TrackYourLifeDotnet.Domain.Primitives;

namespace TrackYourLifeDotnet.Domain.Entities;

public class RefreshToken : Entity
{
    public const int minutesToExpire = 10;

    public RefreshToken(Guid id, string value, Guid userId)
        : base(id)
    {
        Value = value;
        CreatedOn = DateTime.UtcNow;
        ExpiresAt = DateTime.UtcNow.AddMinutes(minutesToExpire);
        UserId = userId;
    }

    public string Value { get; private set; }

    public DateTime CreatedOn { get; }

    public DateTime ExpiresAt { get; private set; }

    [ForeignKey("User")]
    public Guid UserId { get; init; }

    public virtual User User { get; init; } = null!;

    public void UpdateToken(string newValue)
    {
        Value = newValue;
        RefreshLifeTime();
    }

    public void RefreshLifeTime()
    {
        ExpiresAt = DateTime.UtcNow.AddMinutes(minutesToExpire);
    }
}
