using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.Ingredients;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;

public sealed record RecipeReadModel(RecipeId Id, UserId UserId, string Name, bool IsOld)
    : IReadModel<RecipeId>
{
    public List<IngredientReadModel> Ingredients { get; private set; } = [];

    public NutritionalContent NutritionalContents { get; init; } = new NutritionalContent();
}
