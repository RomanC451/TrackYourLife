using System.ComponentModel.DataAnnotations.Schema;
using TrackYourLifeDotnet.Domain.Primitives;

namespace TrackYourLifeDotnet.Domain.Entities;

public class RefreshToken : Entity
{
    public const int minutesToExpire = 1;

    public RefreshToken(Guid id, string value, Guid userId)
        : base(id)
    {
        Value = value;
        CreatedOn = DateTime.UtcNow;
        ExpiresAt = DateTime.UtcNow.AddMinutes(minutesToExpire);
        UserId = userId;
    }

    public string Value { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime ExpiresAt { get; set; }

    [ForeignKey("User")]
    public Guid UserId { get; set; }

    public virtual User User { get; set; } = null!;

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
