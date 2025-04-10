using System.Linq.Expressions;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Data.FoodDiaries.Specifications;

internal sealed class FoodDiaryReadModelWithUserIdAndPeriodSpecification(
    UserId userId,
    DateOnly startDate,
    DateOnly endDate
) : Specification<FoodDiaryReadModel, NutritionDiaryId>
{
    public override Expression<Func<FoodDiaryReadModel, bool>> ToExpression() =>
        foodDiary =>
            foodDiary.UserId == userId && foodDiary.Date >= startDate && foodDiary.Date <= endDate;
}
