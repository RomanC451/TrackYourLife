using TrackYourLife.Common.Contracts.Foods;
using TrackYourLife.Common.Domain.FoodDiaries;
using TrackYourLife.Common.Domain.ServingSizes;

namespace TrackYourLife.Common.Contracts.FoodDiaries;

public sealed record FoodDiaryEntryResponse(
    FoodDiaryEntryId Id,
    FoodResponse Food,
    MealTypes MealType,
    float Quantity,
    ServingSize ServingSize,
    DateOnly EntryDate
);
