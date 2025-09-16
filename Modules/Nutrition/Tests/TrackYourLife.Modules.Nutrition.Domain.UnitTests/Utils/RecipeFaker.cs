using Bogus;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.Ingredients;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;

public static class RecipeFaker
{
    private static readonly Faker f = new();

    public static Recipe Generate(
        RecipeId? id = null,
        UserId? userId = null,
        string? name = null,
        float? weight = null,
        int? portions = null,
        List<Ingredient>? ingredients = null,
        List<Food>? foods = null,
        List<ServingSizeReadModel>? servingSizes = null,
        bool? isOld = null
    )
    {
        if (
            foods != null
            && ingredients != null
            && servingSizes != null
            && foods.Count != ingredients.Count
            && foods.Count != servingSizes.Count
        )
        {
            throw new ArgumentException("Foods and ingredients must have the same count.");
        }

        var recipe = Recipe
            .Create(
                id ?? RecipeId.NewId(),
                userId ?? UserId.NewId(),
                name ?? f.Random.Words(),
                weight ?? 100f,
                portions ?? 1
            )
            .Value;

        if (portions.HasValue)
        {
            recipe.UpdatePortions(portions.Value);
        }

        if (ingredients != null)
        {
            for (int i = 0; i < ingredients.Count; i++)
            {
                recipe.AddIngredient(
                    ingredients[i],
                    foods != null ? foods[i] : FoodFaker.Generate(id: ingredients[i].FoodId),
                    servingSizes != null ? servingSizes[i] : ServingSizeFaker.GenerateReadModel()
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
        float? weight = null,
        bool? isOld = null,
        NutritionalContent? nutritionalContent = null,
        List<IngredientReadModel>? ingredients = null,
        List<ServingSizeReadModel>? servingSizes = null
    )
    {
        return new RecipeReadModel(
            id ?? RecipeId.NewId(),
            userId ?? UserId.NewId(),
            name ?? f.Random.Words(),
            portions ?? 1,
            weight ?? 100f,
            isOld ?? false
        )
        {
            NutritionalContents = nutritionalContent ?? new NutritionalContent(),
            Ingredients = ingredients ?? [IngredientFaker.GenerateReadModel()],
            ServingSizes = servingSizes ?? [],
        };
    }
}
