using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;

namespace TrackYourLife.Modules.Nutrition.Application.Features.FoodDiaries.Queries.GetFoodDiaryById;

public sealed record GetFoodDiaryByIdQuery(NutritionDiaryId DiaryId) : IQuery<FoodDiaryReadModel>;
