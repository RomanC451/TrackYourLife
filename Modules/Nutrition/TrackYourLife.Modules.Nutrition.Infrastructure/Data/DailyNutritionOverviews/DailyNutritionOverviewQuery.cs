using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Nutrition.Domain.Features.DailyNutritionOverviews;
using TrackYourLife.Modules.Nutrition.Infrastructure.Data.DailyNutritionOverviews.Specifications;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Data.DailyNutritionOverviews;

internal sealed class DailyNutritionOverviewQuery(NutritionReadDbContext context)
    : GenericQuery<DailyNutritionOverviewReadModel, DailyNutritionOverviewId>(
        CreateQuery(context.DailyNutritionOverviews)
    ),
        IDailyNutritionOverviewQuery
{
    private static IQueryable<DailyNutritionOverviewReadModel> CreateQuery(
        DbSet<DailyNutritionOverviewReadModel> dbSet
    )
    {
        return dbSet;
    }

    public async Task<DailyNutritionOverviewReadModel?> GetByDateAsync(
        UserId userId,
        DateOnly date,
        CancellationToken cancellationToken
    )
    {
        return await FirstOrDefaultAsync(
            new DailyNutritionOverviewReadModelWithUserIdAndDateSpecification(userId, date),
            cancellationToken
        );
    }

    public async Task<IEnumerable<DailyNutritionOverviewReadModel>> GetByPeriodAsync(
        UserId userId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken
    )
    {
        var list = await WhereAsync(
            new DailyNutritionOverviewReadModelWithUserIdAndPeriodSpecification(
                userId,
                startDate,
                endDate
            ),
            cancellationToken
        );

        return list.OrderBy(x => x.Date);
    }
}
