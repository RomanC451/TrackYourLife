using System.ComponentModel;
using System.Text.Json.Serialization;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Nutrition.Domain.Features.Ingredients;

[JsonConverter(typeof(StronglyTypedGuidJsonConverter<IngredientId>))]
[TypeConverter(typeof(StronglyTypedGuidTypeConverter<IngredientId>))]
public sealed record IngredientId : StronglyTypedGuid<IngredientId>
{
    public IngredientId() { }

    public IngredientId(Guid Value)
        : base(Value) { }

    public static bool TryParse(string? input, out IngredientId? output)
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
