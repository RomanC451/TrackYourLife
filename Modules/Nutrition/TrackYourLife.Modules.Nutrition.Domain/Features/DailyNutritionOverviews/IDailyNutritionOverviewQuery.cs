using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Nutrition.Domain.Features.DailyNutritionOverviews;

public interface IDailyNutritionOverviewQuery
{
    Task<DailyNutritionOverviewReadModel?> GetByDateAsync(
        UserId userId,
        DateOnly date,
        CancellationToken cancellationToken
    );
    Task<IEnumerable<DailyNutritionOverviewReadModel>> GetByPeriodAsync(
        UserId userId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken
    );
}
