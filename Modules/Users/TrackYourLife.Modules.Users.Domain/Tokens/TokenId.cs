using System.ComponentModel;
using System.Text.Json.Serialization;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Users.Domain.Tokens;

[JsonConverter(typeof(StronglyTypedGuidJsonConverter<TokenId>))]
[TypeConverter(typeof(StronglyTypedGuidTypeConverter<TokenId>))]
public sealed record TokenId : StronglyTypedGuid<TokenId>
{
    public TokenId() { }

    public TokenId(Guid Value)
        : base(Value) { }

    public static bool TryParse(string? input, out TokenId? output)
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
