using System.ComponentModel;
using System.Text.Json.Serialization;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Common.Domain.Features.Cookies;

[JsonConverter(typeof(StronglyTypedGuidJsonConverter<CookieId>))]
[TypeConverter(typeof(StronglyTypedGuidTypeConverter<CookieId>))]
public sealed record CookieId : StronglyTypedGuid<CookieId>
{
    public CookieId() { }

    public CookieId(Guid Value)
        : base(Value) { }

    public static bool TryParse(string? input, out CookieId? output)
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
