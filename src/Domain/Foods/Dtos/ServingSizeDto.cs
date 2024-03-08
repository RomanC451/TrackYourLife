using TrackYourLifeDotnet.Domain.Foods.StrongTypes;

namespace TrackYourLifeDotnet.Domain.Foods.Dtos;

public record ServingSizeDto(
    ServingSizeId Id,
    double NutritionMultiplier,
    double Value,
    string Unit,
    int Index
);
