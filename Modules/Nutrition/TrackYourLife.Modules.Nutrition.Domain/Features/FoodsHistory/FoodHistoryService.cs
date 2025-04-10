using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Nutrition.Domain.Features.FoodsHistory;

public sealed class FoodHistoryService : IFoodHistoryService
{
    private const int MaxHistoryItems = 100;
    private readonly IFoodHistoryRepository _foodHistoryRepository;
    private readonly IFoodHistoryQuery _foodHistoryQuery;
    private readonly IFoodQuery _foodQuery;

    public FoodHistoryService(
        IFoodHistoryRepository foodHistoryRepository,
        IFoodHistoryQuery foodHistoryQuery,
        IFoodQuery foodQuery
    )
    {
        _foodHistoryRepository = foodHistoryRepository;
        _foodHistoryQuery = foodHistoryQuery;
        _foodQuery = foodQuery;
    }

    public async Task<Result> AddNewFoodAsync(
        UserId userId,
        FoodId foodId,
        CancellationToken cancellationToken
    )
    {
        var existingHistory = await _foodHistoryRepository.GetByUserAndFoodAsync(
            userId,
            foodId,
            cancellationToken
        );

        if (existingHistory is not null)
        {
            existingHistory.LastUsedNow();
            _foodHistoryRepository.Update(existingHistory);
            return Result.Success();
        }

        var userHistoryCount = await _foodHistoryRepository.GetUserHistoryCountAsync(
            userId,
            cancellationToken
        );

        if (userHistoryCount >= MaxHistoryItems)
        {
            var oldestEntry = await _foodHistoryRepository.GetOldestByUserAsync(
                userId,
                cancellationToken
            );

            if (oldestEntry is not null)
            {
                _foodHistoryRepository.Remove(oldestEntry);
            }
        }

        var newHistoryResult = FoodHistory.Create(FoodHistoryId.NewId(), userId, foodId);

        if (newHistoryResult.IsFailure)
        {
            return Result.Failure(newHistoryResult.Error);
        }

        await _foodHistoryRepository.AddAsync(newHistoryResult.Value, cancellationToken);

        return Result.Success();
    }

    private static double GetTimeOnlyDifferenceInMinutes(DateTime time1, DateTime time2)
    {
        var t1 = TimeOnly.FromDateTime(time1);
        var t2 = TimeOnly.FromDateTime(time2);

        var diffMinutes = Math.Abs((t1.ToTimeSpan() - t2.ToTimeSpan()).TotalMinutes);
        return Math.Min(diffMinutes, 1440 - diffMinutes); // 1440 = minutes in a day
    }

    public async Task<IEnumerable<FoodReadModel>> GetFoodHistoryAsync(
        UserId userId,
        CancellationToken cancellationToken
    )
    {
        var historyItems = await _foodHistoryQuery.GetHistoryByUserAsync(userId, cancellationToken);
        var currentTime = DateTime.UtcNow;
        var foodIds = historyItems.Select(x => x.FoodId).ToList();

        var foods = await _foodQuery.GetFoodsPartOfAsync(foodIds, cancellationToken);

        return foods.OrderBy(x =>
            GetTimeOnlyDifferenceInMinutes(
                historyItems.First(h => h.FoodId == x.Id).LastUsedAt,
                currentTime
            )
        );
    }

    public async Task<IEnumerable<FoodReadModel>> PrioritizeHistoryItemsAsync(
        UserId userId,
        IEnumerable<FoodReadModel> searchResults,
        CancellationToken cancellationToken
    )
    {
        var historyItems = await _foodHistoryQuery.GetHistoryByUserAsync(userId, cancellationToken);
        var currentTime = DateTime.UtcNow;
        var historyIds = historyItems.Select(x => x.FoodId).ToHashSet();

        var historyMatches = searchResults
            .Where(x => historyIds.Contains(x.Id))
            .OrderBy(x =>
                GetTimeOnlyDifferenceInMinutes(
                    historyItems.First(h => h.FoodId == x.Id).LastUsedAt,
                    currentTime
                )
            );

        var nonHistoryMatches = searchResults.Where(x => !historyIds.Contains(x.Id));

        return historyMatches.Concat(nonHistoryMatches);
    }
}
