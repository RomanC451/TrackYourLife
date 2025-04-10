using Bogus;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.Ingredients;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;

public static class RecipeFaker
{
    private static readonly Faker f = new();

    public static Recipe Generate(
        RecipeId? id = null,
        UserId? userId = null,
        string? name = null,
        int? portions = null,
        List<Ingredient>? ingredients = null,
        bool? isOld = null
    )
    {
        var recipe = Recipe
            .Create(id ?? RecipeId.NewId(), userId ?? UserId.NewId(), name ?? f.Random.Words())
            .Value;

        if (portions.HasValue)
        {
            recipe.UpdatePortions(portions.Value);
        }

        if (ingredients != null)
        {
            foreach (var ingredient in ingredients)
            {
                recipe.AddIngredient(
                    ingredient,
                    FoodFaker.Generate(id: ingredient.FoodId),
                    ServingSizeFaker.GenerateReadModel(id: ingredient.ServingSizeId)
                );
            }
        }
        else
        {
            recipe.AddIngredient(
                IngredientFaker.Generate(),
                FoodFaker.Generate(),
                ServingSizeFaker.GenerateReadModel()
            );
        }

        if (isOld == true)
        {
            recipe.MarkAsOld();
        }

        return recipe;
    }

    public static RecipeReadModel GenerateReadModel(
        RecipeId? id = null,
        UserId? userId = null,
        string? name = null,
        int? portions = null,
        bool? isOld = null,
        NutritionalContent? nutritionalContent = null,
        List<IngredientReadModel>? ingredients = null
    )
    {
        return new RecipeReadModel(
            id ?? RecipeId.NewId(),
            userId ?? UserId.NewId(),
            name ?? f.Random.Words(),
            portions ?? 1,
            isOld ?? false
        )
        {
            NutritionalContents = nutritionalContent ?? new NutritionalContent(),
            Ingredients = ingredients ?? [IngredientFaker.GenerateReadModel()],
        };
    }
}
