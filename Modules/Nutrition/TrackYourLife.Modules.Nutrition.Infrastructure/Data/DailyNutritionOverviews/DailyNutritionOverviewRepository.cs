using TrackYourLife.Modules.Nutrition.Domain.Features.DailyNutritionOverviews;
using TrackYourLife.Modules.Nutrition.Infrastructure.Data.DailyNutritionOverviews.Specifications;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Data.DailyNutritionOverviews;

internal sealed class DailyNutritionOverviewRepository(NutritionWriteDbContext context)
    : GenericRepository<DailyNutritionOverview, DailyNutritionOverviewId>(
        context.DailyNutritionOverviews
    ),
        IDailyNutritionOverviewRepository
{
    public async Task<DailyNutritionOverview?> GetByUserIdAndDateAsync(
        UserId userId,
        DateOnly date,
        CancellationToken cancellationToken
    )
    {
        return await FirstOrDefaultAsync(
            new DailyNutritionOverviewWithUserIdAndDateSpecification(userId, date),
            cancellationToken
        );
    }

    public async Task<IEnumerable<DailyNutritionOverview>> GetByUserIdAndDateRangeAsync(
        UserId userId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken
    )
    {
        return await WhereAsync(
            new DailyNutritionOverviewWithUserIdAndPeriodSpecification(userId, startDate, endDate),
            cancellationToken
        );
    }
}
