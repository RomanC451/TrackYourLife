using Bogus;
using TrackYourLife.Modules.Nutrition.Domain.Features.DailyNutritionOverviews;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;

public static class DailyNutritionOverviewFaker
{
    private static readonly Faker f = new();

    public static DailyNutritionOverview Generate(
        DailyNutritionOverviewId? id = null,
        UserId? userId = null,
        DateOnly? date = null,
        float? caloriesGoal = null,
        float? carbohydratesGoal = null,
        float? fatGoal = null,
        float? proteinGoal = null,
        NutritionalContent? nutritionalContent = null
    )
    {
        return DailyNutritionOverview
            .Create(
                id ?? DailyNutritionOverviewId.NewId(),
                userId ?? UserId.NewId(),
                date ?? DateOnly.FromDateTime(DateTime.Today),
                caloriesGoal ?? f.Random.Float(1000, 3000),
                carbohydratesGoal ?? f.Random.Float(100, 400),
                fatGoal ?? f.Random.Float(20, 100),
                proteinGoal ?? f.Random.Float(50, 200)
            )
            .Value;
    }

    public static DailyNutritionOverviewReadModel GenerateReadModel(
        DailyNutritionOverviewId? id = null,
        UserId? userId = null,
        DateOnly? date = null,
        float? caloriesGoal = null,
        float? carbohydratesGoal = null,
        float? fatGoal = null,
        float? proteinGoal = null,
        NutritionalContent? nutritionalContent = null
    )
    {
        return new DailyNutritionOverviewReadModel(
            id ?? DailyNutritionOverviewId.NewId(),
            userId ?? UserId.NewId(),
            date ?? DateOnly.FromDateTime(DateTime.Today),
            caloriesGoal ?? f.Random.Float(1000, 3000),
            carbohydratesGoal ?? f.Random.Float(100, 400),
            fatGoal ?? f.Random.Float(20, 100),
            proteinGoal ?? f.Random.Float(50, 200)
        )
        {
            NutritionalContent =
                nutritionalContent
                ?? new NutritionalContent
                {
                    Energy = new Energy { Value = f.Random.Float(0, 2000), Unit = "Kcal" },
                    Calcium = f.Random.Float(0, 100),
                    Carbohydrates = f.Random.Float(0, 100),
                    Cholesterol = f.Random.Float(0, 100),
                    Fat = f.Random.Float(0, 100),
                    Fiber = f.Random.Float(0, 100),
                    Iron = f.Random.Float(0, 100),
                    MonounsaturatedFat = f.Random.Float(0, 100),
                    NetCarbs = f.Random.Float(0, 100),
                    PolyunsaturatedFat = f.Random.Float(0, 100),
                    Potassium = f.Random.Float(0, 100),
                    Protein = f.Random.Float(0, 100),
                    SaturatedFat = f.Random.Float(0, 100),
                    Sodium = f.Random.Float(0, 100),
                    Sugar = f.Random.Float(0, 100),
                    TransFat = f.Random.Float(0, 100),
                    VitaminA = f.Random.Float(0, 100),
                    VitaminC = f.Random.Float(0, 100),
                },
        };
    }
}
