using System.ComponentModel;
using System.Text.Json.Serialization;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Users.Domain.Features.PersonalAccessTokens;

[JsonConverter(typeof(StronglyTypedGuidJsonConverter<PersonalAccessTokenId>))]
[TypeConverter(typeof(StronglyTypedGuidTypeConverter<PersonalAccessTokenId>))]
public sealed record PersonalAccessTokenId : StronglyTypedGuid<PersonalAccessTokenId>
{
    public PersonalAccessTokenId() { }

    public PersonalAccessTokenId(Guid Value)
        : base(Value) { }

    public static bool TryParse(string? input, out PersonalAccessTokenId? output)
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
