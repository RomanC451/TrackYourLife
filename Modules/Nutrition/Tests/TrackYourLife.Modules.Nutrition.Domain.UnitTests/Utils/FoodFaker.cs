using Bogus;
using NpgsqlTypes;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodServingSizes;

namespace TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;

public static class FoodFaker
{
    private static readonly Faker f = new();

    public static Food Generate(
        FoodId? id = null,
        string? name = null,
        List<FoodServingSize>? foodServingSizes = null,
        int? energyValue = null,
        long? apiId = null
    )
    {
        return Food.Create(
            id ?? FoodId.NewId(),
            "food",
            f.Random.AlphaNumeric(10),
            f.Random.AlphaNumeric(2),
            name ?? f.Random.AlphaNumeric(10),
            new NutritionalContent
            {
                Energy = new Energy { Value = energyValue ?? f.Random.Int(1, 100), Unit = "Kcal" },
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
            foodServingSizes
                ??
                [
                    FoodServingSizeFaker.Generate(0),
                    FoodServingSizeFaker.Generate(1),
                    FoodServingSizeFaker.Generate(2),
                ],
            apiId: apiId ?? f.Random.Long(1, 1000000)
        ).Value;
    }

    public static FoodReadModel GenerateReadModel(
        FoodId? id = null,
        string? name = null,
        string? brandName = null,
        string? countryCode = null,
        List<FoodServingSizeReadModel>? foodServingSizes = null,
        NutritionalContent? nutritionalContent = null,
        int? energyValue = null,
        NpgsqlTsVector? searchVector = null
    )
    {
        return new FoodReadModel(
            id ?? FoodId.NewId(),
            name ?? f.Random.AlphaNumeric(10),
            "food",
            brandName ?? f.Random.AlphaNumeric(10),
            countryCode ?? f.Random.AlphaNumeric(2)
        )
        {
            FoodServingSizes =
                foodServingSizes
                ??
                [
                    FoodServingSizeFaker.GenerateReadModel(0),
                    FoodServingSizeFaker.GenerateReadModel(1),
                    FoodServingSizeFaker.GenerateReadModel(2),
                ],
            NutritionalContents =
                nutritionalContent
                ?? new NutritionalContent
                {
                    Energy = new Energy
                    {
                        Value = energyValue ?? f.Random.Int(1, 100),
                        Unit = "Kcal",
                    },
                    Calcium = 0,
                    Carbohydrates = 0,
                    Cholesterol = 0,
                    Fat = 0,
                    Fiber = 0,
                    Iron = 0,
                    MonounsaturatedFat = 0,
                    NetCarbs = 0,
                    PolyunsaturatedFat = 0,
                    Potassium = 0,
                    Protein = 0,
                    SaturatedFat = 0,
                    Sodium = 0,
                    Sugar = 0,
                    TransFat = 0,
                    VitaminA = 0,
                    VitaminC = 0,
                },
            SearchVector = searchVector ?? null!,
        };
    }
}
