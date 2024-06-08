using TrackYourLife.Common.Domain.Foods;
using TrackYourLife.Common.Domain.ServingSizes;

namespace TrackYourLife.Common.Domain.FoodDiaries;

public sealed record FoodDiaryEntryReadModel(
    Guid Id,
    Guid UserId,
    float Quantity,
    MealTypes MealType,
    DateTime Date,
    DateTime CreatedOnUtc,
    ServingSizeReadModel ServingSize,
    FoodReadModel Food
);
