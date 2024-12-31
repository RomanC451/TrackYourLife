using Bogus;
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
        int? energyValue = null
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
            },
            foodServingSizes
                ??
                [
                    FoodServingSizeFaker.Generate(0),
                    FoodServingSizeFaker.Generate(1),
                    FoodServingSizeFaker.Generate(2)
                ]
        ).Value;
    }

    public static FoodReadModel GenerateReadModel(
        FoodId? id = null,
        List<FoodServingSizeReadModel>? foodServingSizes = null,
        int? energyValue = null
    )
    {
        return new FoodReadModel(
            id ?? FoodId.NewId(),
            f.Random.AlphaNumeric(10),
            "food",
            f.Random.AlphaNumeric(10),
            f.Random.AlphaNumeric(2)
        )
        {
            FoodServingSizes =
                foodServingSizes
                ??
                [
                    FoodServingSizeFaker.GenerateReadModel(0),
                    FoodServingSizeFaker.GenerateReadModel(1),
                    FoodServingSizeFaker.GenerateReadModel(2)
                ],
            NutritionalContents = new NutritionalContent
            {
                Energy = new Energy { Value = energyValue ?? f.Random.Int(1, 100), Unit = "Kcal" },
            },
        };
    }
}
