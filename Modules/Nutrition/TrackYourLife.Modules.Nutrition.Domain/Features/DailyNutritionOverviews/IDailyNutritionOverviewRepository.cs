using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Nutrition.Domain.Features.DailyNutritionOverviews;

public interface IDailyNutritionOverviewRepository
{
    Task<DailyNutritionOverview?> GetByUserIdAndDateAsync(
        UserId userId,
        DateOnly date,
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
