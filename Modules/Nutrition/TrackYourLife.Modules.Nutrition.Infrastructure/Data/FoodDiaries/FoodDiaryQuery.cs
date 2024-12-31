using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Infrastructure.Data.FoodDiaries.Specifications;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Data.FoodDiaries;

internal class FoodDiaryQuery(NutritionReadDbContext context)
    : GenericQuery<FoodDiaryReadModel, NutritionDiaryId>(CreateQuery(context.FoodDiaries)),
        IFoodDiaryQuery
{
    private static IQueryable<FoodDiaryReadModel> CreateQuery(DbSet<FoodDiaryReadModel> dbSet)
    {
        return dbSet
            .Include(de => de.Food)
            .ThenInclude(f => f.FoodServingSizes)
            .ThenInclude(fss => fss.ServingSize)
            .Include(de => de.ServingSize);
    }

    public async Task<IEnumerable<FoodDiaryReadModel>> GetByDateAsync(
        UserId userId,
        DateOnly date,
        CancellationToken cancellationToken
    )
    {
        var list = await WhereAsync(
            new FoodDiaryReadModelWithUserIdAndDateSpecification(userId, date),
            cancellationToken
        );

        return list.OrderBy(de => de.CreatedOnUtc);
    }

    public async Task<IEnumerable<FoodDiaryReadModel>> GetByPeriodAsync(
        UserId userId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken
    )
    {
        var list = await WhereAsync(
            new FoodDiaryReadModelWithUserIdAndPeriodSpecification(userId, startDate, endDate),
            cancellationToken
        );

        return list.OrderBy(de => de.CreatedOnUtc);
    }
}
