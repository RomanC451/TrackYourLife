using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;

public sealed record ServingSizeReadModel(
    ServingSizeId Id,
    float NutritionMultiplier,
    string Unit,
    float Value,
    long? ApiId
) : IReadModel<ServingSizeId>;
