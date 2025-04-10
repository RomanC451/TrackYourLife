using Bogus;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.RecipeDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;

public static class RecipeDiaryFaker
{
    private static readonly Faker f = new();

    public static RecipeDiary Generate(
        NutritionDiaryId? id = null,
        UserId? userId = null,
        RecipeId? recipeId = null,
        float? quantity = null,
        DateOnly? date = null,
        MealTypes? mealType = null
    )
    {
        return RecipeDiary
            .Create(
                id ?? NutritionDiaryId.NewId(),
                userId ?? UserId.NewId(),
                recipeId ?? RecipeId.NewId(),
                quantity ?? f.Random.Int(1, 100),
                date ?? DateOnly.FromDateTime(DateTime.UtcNow),
                mealType ?? MealTypes.Breakfast
            )
            .Value;
    }

    public static RecipeDiaryReadModel GenerateReadModel(
        NutritionDiaryId? id = null,
        UserId? userId = null,
        RecipeReadModel? recipe = null,
        float? quantity = null,
        DateOnly? date = null,
        MealTypes? mealType = null,
        DateTime? createdOnUtc = null
    )
    {
        return new RecipeDiaryReadModel(
            id ?? NutritionDiaryId.NewId(),
            userId ?? UserId.NewId(),
            quantity ?? f.Random.Int(1, 100),
            mealType ?? MealTypes.Breakfast,
            date ?? DateOnly.FromDateTime(DateTime.UtcNow),
            createdOnUtc ?? DateTime.UtcNow
        )
        {
            Recipe =
                recipe
                ?? new RecipeReadModel(
                    RecipeId.NewId(),
                    userId ?? UserId.NewId(),
                    f.Random.Words(),
                    1,
                    false
                ),
        };
    }
}
