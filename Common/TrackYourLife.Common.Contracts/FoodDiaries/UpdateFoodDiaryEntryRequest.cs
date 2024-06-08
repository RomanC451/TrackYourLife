using System.ComponentModel.DataAnnotations;
using TrackYourLife.Common.Domain.FoodDiaries;
using TrackYourLife.Common.Domain.ServingSizes;

namespace TrackYourLife.Common.Contracts.FoodDiaries;

public sealed record UpdateFoodDiaryEntryRequest(
    FoodDiaryEntryId Id,
    float Quantity,
    ServingSizeId ServingSizeId,
    MealTypes MealType
);
