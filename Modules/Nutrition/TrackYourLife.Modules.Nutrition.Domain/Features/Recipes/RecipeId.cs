using System.ComponentModel;
using System.Text.Json.Serialization;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;

[JsonConverter(typeof(StronglyTypedGuidJsonConverter<RecipeId>))]
[TypeConverter(typeof(StronglyTypedGuidTypeConverter<RecipeId>))]
public sealed record RecipeId : StronglyTypedGuid<RecipeId>
{
    public RecipeId() { }

    public RecipeId(Guid Value)
        : base(Value) { }

    public static bool TryParse(string? input, out RecipeId? output)
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
