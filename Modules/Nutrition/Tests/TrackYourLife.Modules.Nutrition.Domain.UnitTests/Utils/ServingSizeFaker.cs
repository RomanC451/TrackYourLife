using Bogus;
using MediatR;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;

namespace TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;

public static class ServingSizeFaker
{
    private static readonly Faker f = new();

    private static readonly List<string> units = ["g", "ml", "oz", "lb", "cup", "tbsp"];

    public static ServingSize Generate(
        ServingSizeId? id = null,
        float? nutritionMultiplier = null,
        string? unit = null,
        float? value = null
    )
    {
        return ServingSize
            .Create(
                id ?? ServingSizeId.NewId(),
                nutritionMultiplier ?? f.Random.Int(1, 10),
                unit ?? f.Random.ListItem(units),
                value ?? f.Random.Int(1, 100)
            )
            .Value;
    }

    public static ServingSizeReadModel GenerateReadModel(
        ServingSizeId? id = null,
        float? nutritionMultiplier = null,
        string? unit = null,
        float? value = null
    )
    {
        return new ServingSizeReadModel(
            id ?? ServingSizeId.NewId(),
            nutritionMultiplier ?? f.Random.Int(1, 10),
            unit ?? f.Random.ListItem(units),
            value ?? f.Random.Int(1, 100)
        );
    }
}
