using TrackYourLife.Common.Domain.ServingSizes;

namespace TrackYourLife.Common.Contracts.Foods;

public record ServingSizeResponse(
    ServingSizeId Id,
    double NutritionMultiplier,
    double Value,
    string Unit,
    int Index
);
