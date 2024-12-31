using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Domain.Utils;

namespace TrackYourLife.Modules.Nutrition.Domain.Features.FoodsHistory;

public sealed class FoodHistory : Entity<FoodHistoryId>
{
    public override FoodHistoryId Id { get; set; } = FoodHistoryId.Empty;
    public UserId UserId { get; private set; } = UserId.Empty;
    public FoodId FoodId { get; private set; } = FoodId.Empty;
    public DateTime LastUsedAt { get; private set; }

    private FoodHistory()
        : base() { }

    private FoodHistory(FoodHistoryId id, UserId userId, FoodId foodId, DateTime lastUsedAt)
        : base(id)
    {
        UserId = userId;
        FoodId = foodId;
        LastUsedAt = lastUsedAt;
    }

    public static Result<FoodHistory> Create(FoodHistoryId id, UserId userId, FoodId foodId)
    {
        var result = Result.FirstFailureOrSuccess(
            Ensure.NotEmptyId(
                id,
                DomainErrors.ArgumentError.Empty(nameof(FoodHistory), nameof(id))
            ),
            Ensure.NotEmptyId(
                userId,
                DomainErrors.ArgumentError.Empty(nameof(FoodHistory), nameof(userId))
            ),
            Ensure.NotEmptyId(
                foodId,
                DomainErrors.ArgumentError.Empty(nameof(FoodHistory), nameof(foodId))
            )
        );

        if (result.IsFailure)
        {
            return Result.Failure<FoodHistory>(result.Error);
        }

        return Result.Success(new FoodHistory(id, userId, foodId, DateTime.UtcNow));
    }

    public void UpdateLastUsedAt()
    {
        LastUsedAt = DateTime.UtcNow;
    }
}
