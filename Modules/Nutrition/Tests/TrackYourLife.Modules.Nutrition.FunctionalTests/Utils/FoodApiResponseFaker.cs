using Bogus;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Infrastructure.Services.FoodApi;

namespace TrackYourLife.Modules.Nutrition.FunctionalTests.Utils;

public static class FoodApiResponseFaker
{
    private static readonly Faker f = new();

    public static FoodApiResponse Generate(int count = 1, string? foodName = null)
    {
        return new FoodApiResponse { Items = GenerateItems(count, foodName) };
    }

    private static List<ItemListElement> GenerateItems(int count, string? foodName)
    {
        return Enumerable
            .Range(0, count)
            .Select(i => new ItemListElement
            {
                Item = new ApiFood
                {
                    Id = f.Random.Long(1, 1000000),
                    Type = "food",
                    BrandName = f.Company.CompanyName(),
                    CountryCode = f.Address.CountryCode(),
                    Description = foodName + " " + f.Commerce.ProductName(),
                    NutritionalContents = new NutritionalContent
                    {
                        Energy = new Energy { Value = f.Random.Int(1, 1000), Unit = "Kcal" },
                        Calcium = f.Random.Int(0, 100),
                        Carbohydrates = f.Random.Int(0, 100),
                        Cholesterol = f.Random.Int(0, 100),
                        Fat = f.Random.Int(0, 100),
                        Fiber = f.Random.Int(0, 100),
                        Iron = f.Random.Int(0, 100),
                        MonounsaturatedFat = f.Random.Int(0, 100),
                        NetCarbs = f.Random.Int(0, 100),
                        PolyunsaturatedFat = f.Random.Int(0, 100),
                        Potassium = f.Random.Int(0, 100),
                        Protein = f.Random.Int(0, 100),
                        SaturatedFat = f.Random.Int(0, 100),
                        Sodium = f.Random.Int(0, 100),
                        Sugar = f.Random.Int(0, 100),
                        TransFat = f.Random.Int(0, 100),
                        VitaminA = f.Random.Int(0, 100),
                        VitaminC = f.Random.Int(0, 100),
                    },
                    ServingSizes = new List<ApiServingSize>
                    {
                        new()
                        {
                            Id = f.Random.Long(1, 1000000),
                            NutritionMultiplier = f.Random.Float(0.1f, 10f),
                            Unit = f.PickRandom("g", "ml", "oz", "cup"),
                            Value = f.Random.Float(1f, 1000f),
                        },
                    },
                },
            })
            .ToList();
    }
}
