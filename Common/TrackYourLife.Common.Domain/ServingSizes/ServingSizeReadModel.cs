namespace TrackYourLife.Common.Domain.ServingSizes;

public sealed record ServingSizeReadModel(
    Guid Id,
    double NutritionMultiplier,
    string Unit,
    double Value
);
