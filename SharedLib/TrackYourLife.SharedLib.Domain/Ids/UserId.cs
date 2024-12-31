using System.ComponentModel;
using System.Text.Json.Serialization;

namespace TrackYourLife.SharedLib.Domain.Ids;

[JsonConverter(typeof(StronglyTypedGuidJsonConverter<UserId>))]
[TypeConverter(typeof(StronglyTypedGuidTypeConverter<UserId>))]
public sealed record UserId : StronglyTypedGuid<UserId>
{
    public UserId() { }

    public UserId(Guid Value)
        : base(Value) { }

    public static bool TryParse(string? input, out UserId? output)
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
