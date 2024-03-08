using TrackYourLifeDotnet.Domain.Users.StrongTypes;

namespace TrackYourLifeDotnet.Domain.FoodDiaries.Queries;

public interface IFoodDiaryQuery
{
    Task<List<FoodDiaryEntry>> GetDailyFoodDiaryQuery(
        UserId userId,
        DateOnly date,
        CancellationToken cancellationToken
    );
}
