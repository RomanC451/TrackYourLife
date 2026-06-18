using System.ComponentModel;
using System.Text.Json.Serialization;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Reading.Domain.Features.Books;

[JsonConverter(typeof(StronglyTypedGuidJsonConverter<BookId>))]
[TypeConverter(typeof(StronglyTypedGuidTypeConverter<BookId>))]
public sealed record BookId : StronglyTypedGuid<BookId>
{
    public BookId() { }

    public BookId(Guid Value)
        : base(Value) { }

    public static bool TryParse(string? input, out BookId? output)
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

    public override string ToString() => Value.ToString();
}
