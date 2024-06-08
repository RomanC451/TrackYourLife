using Microsoft.EntityFrameworkCore;
using TrackYourLife.Common.Domain.FoodDiaries;
using TrackYourLife.Common.Domain.Users;


namespace TrackYourLife.Common.Persistence.Queries;

internal class FoodDiaryQuery(ApplicationWriteDbContext context) : IFoodDiaryQuery
{
    private readonly ApplicationWriteDbContext _context = context;

    public async Task<List<FoodDiaryEntry>> GetFoodDiaryByDateQuery(
        UserId userId,
        DateOnly date,
        CancellationToken cancellationToken
    )
    {
        return await _context.FoodDiaries
            .Include(de => de.Food)
            .Include(de => de.ServingSize)
            .Where(de => de.Date == date && de.UserId == userId)
            .OrderBy(de => de.CreatedOnUtc)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetTotalCaloriesByPeriodQuery(
        UserId userId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken
    )
    {
        return await _context.FoodDiaries
            .Include(de => de.Food)
            .Include(de => de.ServingSize)
            .Where(de => de.Date >= startDate && de.Date <= endDate && de.UserId == userId)
            .SumAsync(
                de =>
                    (int)(
                        de.Food.NutritionalContent.Energy.Value
                        * de.ServingSize.NutritionMultiplier
                        * de.Quantity
                    ),
                cancellationToken
            );
    }
}
