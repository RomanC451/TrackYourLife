using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;

namespace TrackYourLife.Modules.Nutrition.Application.Features.RecipeDiaries.Queries.GetRecipeDiaryById;

public sealed record GetFoodDiaryByIdQuery(NutritionDiaryId DiaryId) : IQuery<FoodDiaryReadModel>;
