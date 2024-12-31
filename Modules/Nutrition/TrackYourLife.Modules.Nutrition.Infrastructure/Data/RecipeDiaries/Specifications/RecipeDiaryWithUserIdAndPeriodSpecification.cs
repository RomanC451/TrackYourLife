using System.Linq.Expressions;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.RecipeDiaries;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Data.RecipeDiaries.Specifications;

public class RecipeDiaryReadModelWithUserIdAndPeriodSpecification(
    UserId userId,
    DateOnly startDate,
    DateOnly endDate
) : Specification<RecipeDiaryReadModel, NutritionDiaryId>
{
    public override Expression<Func<RecipeDiaryReadModel, bool>> ToExpression() =>
        recipeDiary =>
            recipeDiary.UserId == userId
            && recipeDiary.Date >= startDate
            && recipeDiary.Date <= endDate;
}
