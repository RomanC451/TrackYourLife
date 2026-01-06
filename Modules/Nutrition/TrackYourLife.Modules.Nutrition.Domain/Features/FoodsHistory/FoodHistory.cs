using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Domain.Utils;

namespace TrackYourLife.Modules.Nutrition.Domain.Features.FoodsHistory;

public sealed class FoodHistory : Entity<FoodHistoryId>
{
    public UserId UserId { get; private set; } = UserId.Empty;
    public FoodId FoodId { get; private set; } = FoodId.Empty;
    public DateTime LastUsedAt { get; private set; }
    public ServingSizeId LastServingSizeUsedId { get; private set; } = ServingSizeId.Empty;
    public float LastQuantityUsed { get; private set; }

    private FoodHistory()
        : base() { }

    private FoodHistory(
        FoodHistoryId id,
        UserId userId,
        FoodId foodId,
        DateTime lastUsedAt,
        ServingSizeId lastServingSizeUsedId,
        float lastQuantityUsed
    )
        : base(id)
    {
        UserId = userId;
        FoodId = foodId;
        LastUsedAt = lastUsedAt;
        LastServingSizeUsedId = lastServingSizeUsedId;
        LastQuantityUsed = lastQuantityUsed;
    }

    public static Result<FoodHistory> Create(
        FoodHistoryId id,
        UserId userId,
        FoodId foodId,
        ServingSizeId lastServingSizeUsedId,
        float lastQuantityUsed
    )
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
            ),
            Ensure.NotEmptyId(
                lastServingSizeUsedId,
                DomainErrors.ArgumentError.Empty(nameof(FoodHistory), nameof(lastServingSizeUsedId))
            ),
            Ensure.Positive(
                lastQuantityUsed,
                DomainErrors.ArgumentError.NotPositive(
                    nameof(FoodHistory),
                    nameof(lastQuantityUsed)
                )
            )
        );

        if (result.IsFailure)
        {
            return Result.Failure<FoodHistory>(result.Error);
        }

        return Result.Success(
            new FoodHistory(
                id,
                userId,
                foodId,
                DateTime.UtcNow,
                lastServingSizeUsedId,
                lastQuantityUsed
            )
        );
    }

    public void LastUsedNow(ServingSizeId servingSizeId, float quantity)
    {
        LastUsedAt = DateTime.UtcNow;
        LastServingSizeUsedId = servingSizeId;
        LastQuantityUsed = quantity;
    }
}
