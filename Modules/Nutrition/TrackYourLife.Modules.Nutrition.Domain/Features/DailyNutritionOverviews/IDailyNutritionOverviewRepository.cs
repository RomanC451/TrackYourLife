using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Nutrition.Domain.Features.DailyNutritionOverviews;

public interface IDailyNutritionOverviewRepository
{
    Task<DailyNutritionOverview?> GetByUserIdAndDateAsync(
        UserId userId,
        DateOnly date,
        CancellationToken cancellationToken
    );

    Task<IEnumerable<DailyNutritionOverview>> GetByUserIdAndDateRangeAsync(
        UserId userId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken
    );

    Task AddAsync(
        DailyNutritionOverview dailyNutritionOverview,
        CancellationToken cancellationToken
    );

    Task AddRangeAsync(
        IEnumerable<DailyNutritionOverview> dailyNutritionOverviews,
        CancellationToken cancellationToken
    );

    void Update(DailyNutritionOverview dailyNutritionOverview);
}
