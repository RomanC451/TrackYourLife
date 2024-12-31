using System.ComponentModel;
using System.Text.Json.Serialization;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Users.Domain.Goals;

[JsonConverter(typeof(StronglyTypedGuidJsonConverter<GoalId>))]
[TypeConverter(typeof(StronglyTypedGuidTypeConverter<GoalId>))]
public sealed record GoalId : StronglyTypedGuid<GoalId>
{
    public GoalId() { }

    public GoalId(Guid Value)
        : base(Value) { }

    public static bool TryParse(string? input, out GoalId? output)
    {
        output = null;

        if (string.IsNullOrEmpty(input))
        {
            return false;
        }

        if (!Guid.TryParse(input, out var guid))
        {
            return false;
        }

        output = Create(guid);

        return true;
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}
