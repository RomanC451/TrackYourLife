using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries;

public sealed record FoodDiaryReadModel(
    NutritionDiaryId Id,
    UserId UserId,
    float Quantity,
    MealTypes MealType,
    DateOnly Date,
    DateTime CreatedOnUtc
) : NutritionDiaryReadModel(Id, UserId, Quantity, MealType, Date, CreatedOnUtc)
{
    public required FoodReadModel Food { get; init; }
    public required ServingSizeReadModel ServingSize { get; init; }
}
