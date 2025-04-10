using Bogus;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.Ingredients;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;

public static class IngredientFaker
{
    private static readonly Faker f = new();

    public static Ingredient Generate(
        IngredientId? id = null,
        UserId? userId = null,
        FoodId? foodId = null,
        ServingSizeId? servingSizeId = null,
        float? quantity = null
    )
    {
        var ingredient = Ingredient
            .Create(
                userId ?? UserId.NewId(),
                id ?? IngredientId.NewId(),
                foodId ?? FoodId.NewId(),
                servingSizeId ?? ServingSizeId.NewId(),
                quantity ?? f.Random.Float(1, 100)
            )
            .Value;

        return ingredient;
    }

    public static IngredientReadModel GenerateReadModel(
        IngredientId? id = null,
        FoodReadModel? food = null,
        ServingSizeReadModel? servingSize = null,
        float? quantity = null,
        DateTime? createdOnUtc = null
    )
    {
        return new IngredientReadModel(
            id ?? IngredientId.NewId(),
            quantity ?? f.Random.Float(1, 100),
            createdOnUtc ?? DateTime.UtcNow
        )
        {
            Food = food ?? FoodFaker.GenerateReadModel(),
            ServingSize = servingSize ?? ServingSizeFaker.GenerateReadModel(),
        };
    }
}
