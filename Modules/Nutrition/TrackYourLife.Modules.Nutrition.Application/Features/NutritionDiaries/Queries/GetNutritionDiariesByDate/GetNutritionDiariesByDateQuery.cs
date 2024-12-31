using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.RecipeDiaries;

namespace TrackYourLife.Modules.Nutrition.Application.Features.NutritionDiaries.Queries.GetNutritionDiariesByDate;

public sealed record GetNutritionDiariesByDateQuery(DateOnly Day)
    : IQuery<(
        Dictionary<MealTypes, List<FoodDiaryReadModel>>,
        Dictionary<MealTypes, List<RecipeDiaryReadModel>>
    )>;
