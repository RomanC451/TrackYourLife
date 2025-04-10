using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries.Events;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Domain.Utils;

namespace TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries;

public sealed class FoodDiary : NutritionDiary
{
    public FoodId FoodId { get; private set; } = FoodId.Empty;
    public ServingSizeId ServingSizeId { get; private set; } = ServingSizeId.Empty;

    private FoodDiary()
        : base() { }

    private FoodDiary(
        NutritionDiaryId id,
        UserId userId,
        FoodId foodId,
        float quantity,
        DateOnly date,
        MealTypes mealType,
        ServingSizeId servingSizeId
    )
        : base(id, userId, quantity, date, mealType)
    {
        FoodId = foodId;
        ServingSizeId = servingSizeId;
    }

    public static Result<FoodDiary> Create(
        NutritionDiaryId id,
        UserId userId,
        FoodId foodId,
        float quantity,
        DateOnly date,
        MealTypes mealType,
        ServingSizeId servingSizeId
    )
    {
        var result = Result.FirstFailureOrSuccess(
            Ensure.NotEmptyId(id, DomainErrors.ArgumentError.Empty(nameof(FoodDiary), nameof(id))),
            Ensure.NotEmptyId(
                userId,
                DomainErrors.ArgumentError.Empty(nameof(FoodDiary), nameof(userId))
            ),
            Ensure.NotEmptyId(
                foodId,
                DomainErrors.ArgumentError.Empty(nameof(FoodDiary), nameof(foodId))
            ),
            Ensure.Positive(
                quantity,
                DomainErrors.ArgumentError.NotPositive(nameof(FoodDiary), nameof(quantity))
            ),
            Ensure.NotEmpty(
                date,
                DomainErrors.ArgumentError.Empty(nameof(FoodDiary), nameof(date))
            ),
            Ensure.NotEmptyId(
                servingSizeId,
                DomainErrors.ArgumentError.Empty(nameof(FoodDiary), nameof(servingSizeId))
            ),
            Ensure.IsInEnum(
                mealType,
                DomainErrors.ArgumentError.Invalid(nameof(FoodDiary), nameof(mealType))
            )
        );

        if (result.IsFailure)
        {
            return Result.Failure<FoodDiary>(result.Error);
        }

        var foodDiary = new FoodDiary(id, userId, foodId, quantity, date, mealType, servingSizeId);

        foodDiary.RaiseDirectDomainEvent(
            new FoodDiaryCreatedDomainEvent(userId, foodId, date, servingSizeId, quantity)
        );

        return Result.Success(foodDiary);
    }

    public Result UpdateServingSizeId(ServingSizeId servingSizeId)
    {
        var result = Result.FirstFailureOrSuccess(
            Ensure.NotEmptyId(
                servingSizeId,
                DomainErrors.ArgumentError.Empty(nameof(FoodDiary), nameof(servingSizeId))
            )
        );

        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }

        ServingSizeId = servingSizeId;

        return Result.Success();
    }

    public override void OnDelete()
    {
        RaiseDirectDomainEvent(
            new FoodDiaryDeletedDomainEvent(UserId, FoodId, ServingSizeId, Date, Quantity)
        );
    }
}
