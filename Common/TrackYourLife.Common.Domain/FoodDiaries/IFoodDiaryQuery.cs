using TrackYourLife.Common.Domain.Users;

namespace TrackYourLife.Common.Domain.FoodDiaries;

public interface IFoodDiaryQuery
{
    Task<List<FoodDiaryEntry>> GetFoodDiaryByDateQuery(
        UserId userId,
        DateOnly date,
        CancellationToken cancellationToken
    );

    Task<int> GetTotalCaloriesByPeriodQuery(
        UserId userId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken
    );
}
