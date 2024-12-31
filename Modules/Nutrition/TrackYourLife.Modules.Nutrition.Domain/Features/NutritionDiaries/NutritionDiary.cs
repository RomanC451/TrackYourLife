using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Domain.Utils;

namespace TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;

public abstract class NutritionDiary : AggregateRoot<NutritionDiaryId>, IAuditableEntity
{
    public UserId UserId { get; private set; } = UserId.Empty;
    public float Quantity { get; private set; }
    public DateOnly Date { get; private set; }
    public MealTypes MealType { get; private set; }
    public DateTime CreatedOnUtc { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedOnUtc { get; set; } = null;

    protected NutritionDiary()
        : base() { }

    protected NutritionDiary(
        NutritionDiaryId id,
        UserId userId,
        float quantity,
        DateOnly date,
        MealTypes mealType
    )
        : base(id)
    {
        UserId = userId;
        Quantity = quantity;
        Date = date;
        MealType = mealType;
    }

    public Result UpdateQuantity(float quantity)
    {
        var result = Ensure.Positive(
            quantity,
            DomainErrors.ArgumentError.NotPositive(nameof(FoodDiary), nameof(quantity))
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
            DomainErrors.ArgumentError.Invalid(nameof(FoodDiary), nameof(mealType))
        );

        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }

        MealType = mealType;

        return Result.Success();
    }
}
