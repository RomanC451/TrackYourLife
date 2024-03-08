using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using TrackYourLifeDotnet.Domain.FoodDiaries;
using TrackYourLifeDotnet.Domain.FoodDiaries.Queries;
using TrackYourLifeDotnet.Domain.Users.StrongTypes;

namespace TrackYourLifeDotnet.Persistence.Queries;

public class FoodDiaryQuery : IFoodDiaryQuery
{
    private readonly ApplicationDbContext _context;

    public FoodDiaryQuery(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<FoodDiaryEntry>> GetDailyFoodDiaryQuery(
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
}
