using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Domain.Utils;

namespace TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;

public abstract class NutritionDiary : AggregateRoot<NutritionDiaryId>, IAuditableEntity
{
    public UserId UserId { get; private set; } = UserId.Empty;
    public ServingSizeId ServingSizeId { get; private set; } = ServingSizeId.Empty;
    public float Quantity { get; private set; }
    public DateOnly Date { get; private set; }
    public MealTypes MealType { get; private set; }
    public DateTime CreatedOnUtc { get; private set; }
    public DateTime? ModifiedOnUtc { get; }

    protected NutritionDiary()
        : base() { }

    protected NutritionDiary(
        NutritionDiaryId id,
        UserId userId,
        float quantity,
        DateOnly date,
        MealTypes mealType,
        ServingSizeId servingSizeId
    )
        : base(id)
    {
        UserId = userId;
        Quantity = quantity;
        Date = date;
        MealType = mealType;
        ServingSizeId = servingSizeId;
        CreatedOnUtc = DateTime.UtcNow;
    }

    public Result UpdateQuantity(float quantity)
    {
        var result = Ensure.Positive(
            quantity,
            DomainErrors.ArgumentError.NotPositive(nameof(NutritionDiary), nameof(quantity))
        );

        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }

        Quantity = quantity;

        return Result.Success();
    }

    public Result UpdateMealType(MealTypes mealType)
    {
        var result = Ensure.IsInEnum(
            mealType,
            DomainErrors.ArgumentError.Invalid(nameof(NutritionDiary), nameof(mealType))
        );

        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }

        MealType = mealType;

        return Result.Success();
    }

    public Result UpdateEntryDate(DateOnly entryDate)
    {
        var result = Ensure.NotEmpty(
            entryDate,
            DomainErrors.ArgumentError.Empty(nameof(NutritionDiary), nameof(entryDate))
        );

        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }

        Date = entryDate;

        return Result.Success();
    }

    public Result UpdateServingSizeId(ServingSizeId servingSizeId)
    {
        var result = Result.FirstFailureOrSuccess(
            Ensure.NotEmptyId(
                servingSizeId,
                DomainErrors.ArgumentError.Empty(nameof(NutritionDiary), nameof(servingSizeId))
            )
        );

        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }

        ServingSizeId = servingSizeId;

        return Result.Success();
    }
}
