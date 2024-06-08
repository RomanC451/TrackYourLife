using TrackYourLife.Common.Domain.FoodDiaries;
using TrackYourLife.Common.Domain.Foods;
using TrackYourLife.Common.Domain.ServingSizes;

namespace TrackYourLife.Common.Contracts.FoodDiaries;

public sealed record AddFoodDiaryEntryRequest(
    FoodId FoodId,
    MealTypes MealType,
    ServingSizeId ServingSizeId,
    float Quantity,
    DateOnly EntryDate
);
