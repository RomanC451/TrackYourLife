using System.Text.Json;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.Ingredients;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;

public sealed record RecipeReadModel(
    RecipeId Id,
    UserId UserId,
    string Name,
    int Portions,
    float Weight,
    bool IsOld,
    string ServingSizesJson = "[]"
) : IReadModel<RecipeId>
{
    public List<IngredientReadModel> Ingredients { get; init; } = [];

    public NutritionalContent NutritionalContents { get; init; } = new NutritionalContent();

    public List<ServingSizeReadModel> ServingSizes
    {
        get => JsonSerializer.Deserialize<List<ServingSizeReadModel>>(ServingSizesJson) ?? [];
        init => ServingSizesJson = JsonSerializer.Serialize(value);
    }
}
