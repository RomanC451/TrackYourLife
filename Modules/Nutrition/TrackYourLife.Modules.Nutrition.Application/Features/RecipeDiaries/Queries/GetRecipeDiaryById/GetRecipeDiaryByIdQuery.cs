using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.RecipeDiaries;

namespace TrackYourLife.Modules.Nutrition.Application.Features.RecipeDiaries.Queries.GetRecipeDiaryById;

public sealed record GetRecipeDiaryByIdQuery(NutritionDiaryId DiaryId)
    : IQuery<RecipeDiaryReadModel>;
