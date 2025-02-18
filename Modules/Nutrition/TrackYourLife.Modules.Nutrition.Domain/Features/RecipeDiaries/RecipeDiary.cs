using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.RecipeDiaries.DomainEvents;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Domain.Utils;

namespace TrackYourLife.Modules.Nutrition.Domain.Features.RecipeDiaries;

public class RecipeDiary : NutritionDiary
{
    public RecipeId RecipeId { get; private set; } = RecipeId.Empty;

    private RecipeDiary()
        : base() { }

    private RecipeDiary(
        NutritionDiaryId id,
        UserId userId,
        RecipeId recipeId,
        float quantity,
        DateOnly date,
        MealTypes mealType
    )
        : base(id, userId, quantity, date, mealType)
    {
        RecipeId = recipeId;
    }

    public static Result<RecipeDiary> Create(
        NutritionDiaryId id,
        UserId userId,
        RecipeId recipeId,
        float quantity,
        DateOnly date,
        MealTypes mealType
    )
    {
        var result = Result.FirstFailureOrSuccess(
            Ensure.NotEmptyId(
                id,
                DomainErrors.ArgumentError.Empty(nameof(RecipeDiary), nameof(id))
            ),
            Ensure.NotEmptyId(
                userId,
                DomainErrors.ArgumentError.Empty(nameof(RecipeDiary), nameof(userId))
            ),
            Ensure.NotEmptyId(
                recipeId,
                DomainErrors.ArgumentError.Empty(nameof(RecipeDiary), nameof(recipeId))
            ),
            Ensure.Positive(
                quantity,
                DomainErrors.ArgumentError.NotPositive(nameof(RecipeDiary), nameof(quantity))
            ),
            Ensure.NotEmpty(
                date,
                DomainErrors.ArgumentError.Empty(nameof(RecipeDiary), nameof(date))
            ),
            Ensure.IsInEnum(
                mealType,
                DomainErrors.ArgumentError.Invalid(nameof(RecipeDiary), nameof(mealType))
            )
        );

        if (result.IsFailure)
        {
            return Result.Failure<RecipeDiary>(result.Error);
        }

        var recipeDiary = new RecipeDiary(id, userId, recipeId, quantity, date, mealType);

        recipeDiary.RaiseDomainEvent(
            new RecipeDiaryCreatedDomainEvent(userId, recipeId, date, quantity)
        );

        return Result.Success(recipeDiary);
    }

    public override void OnDelete()
    {
        this.RaiseDomainEvent(new RecipeDiaryDeletedDomainEvent(UserId, Date, RecipeId, Quantity));
    }
}
