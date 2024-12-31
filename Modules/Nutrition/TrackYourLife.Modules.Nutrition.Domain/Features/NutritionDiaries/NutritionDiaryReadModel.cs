using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;

public abstract record NutritionDiaryReadModel(
    NutritionDiaryId Id,
    UserId UserId,
    float Quantity,
    MealTypes MealType,
    DateOnly Date,
    DateTime CreatedOnUtc
) : IReadModel<NutritionDiaryId>;
