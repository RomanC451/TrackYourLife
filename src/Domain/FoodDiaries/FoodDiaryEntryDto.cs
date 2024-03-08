using TrackYourLifeDotnet.Domain.Foods;

namespace TrackYourLifeDotnet.Domain.FoodDiaries;

public sealed record FoodDiaryEntryDto(
    FoodDiaryEntryId Id,
    Food Food,
    MealTypes MealType,
    float Quantity,
    ServingSize ServingSize,
    DateOnly Date
);
