using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.Ingredients.Events;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Domain.Utils;

namespace TrackYourLife.Modules.Nutrition.Domain.Features.Ingredients;

public sealed class Ingredient : AggregateRoot<IngredientId>, IAuditableEntity
{
    public FoodId FoodId { get; private set; } = FoodId.Empty;
    public ServingSizeId ServingSizeId { get; private set; } = null!;
    public float Quantity { get; private set; }
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }

    private Ingredient()
        : base() { }

    private Ingredient(
        IngredientId id,
        FoodId foodId,
        ServingSizeId servingSizeId,
        float quantity,
        DateTime createdOnUtc
    )
        : base(id)
    {
        FoodId = foodId;
        ServingSizeId = servingSizeId;
        Quantity = quantity;
        CreatedOnUtc = createdOnUtc;
    }

    public static Result<Ingredient> Create(
        UserId userId,
        IngredientId id,
        FoodId foodId,
        ServingSizeId servingSizeId,
        float quantity
    )
    {
        var result = Result.FirstFailureOrSuccess(
            Ensure.NotEmptyId(id, DomainErrors.ArgumentError.Empty(nameof(Ingredient), nameof(id))),
            Ensure.NotNull(
                foodId,
                DomainErrors.ArgumentError.Null(nameof(Ingredient), nameof(foodId))
            ),
            Ensure.NotNull(
                servingSizeId,
                DomainErrors.ArgumentError.Null(nameof(Ingredient), nameof(servingSizeId))
            ),
            Ensure.Positive(
                quantity,
                DomainErrors.ArgumentError.NotPositive(nameof(Ingredient), nameof(quantity))
            )
        );

        if (result.IsFailure)
        {
            return Result.Failure<Ingredient>(result.Error);
        }

        var ingredient = new Ingredient(id, foodId, servingSizeId, quantity, DateTime.UtcNow);

        ingredient.RaiseDomainEvent(new IngredientCreatedDomainEvent(userId, foodId));

        return Result.Success(ingredient);
    }

    public Result<Ingredient> Clone(IngredientId id)
    {
        var result = Result.FirstFailureOrSuccess(
            Ensure.NotEmptyId(id, DomainErrors.ArgumentError.Empty(nameof(Ingredient), nameof(id)))
        );

        if (result.IsFailure)
        {
            return Result.Failure<Ingredient>(result.Error);
        }

        return Result.Success(new Ingredient(id, FoodId, ServingSizeId, Quantity, CreatedOnUtc));
    }

    public Result UpdateServingSize(ServingSizeId servingSizeId)
    {
        var result = Result.FirstFailureOrSuccess(
            Ensure.NotNull(
                servingSizeId,
                DomainErrors.ArgumentError.Null(nameof(Ingredient), nameof(servingSizeId))
            )
        );

        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }

        ServingSizeId = servingSizeId;

        ModifiedOnUtc = DateTime.UtcNow;

        return Result.Success();
    }

    public Result UpdateQuantity(float quantity)
    {
        var result = Ensure.Positive(
            quantity,
            DomainErrors.ArgumentError.NotPositive(nameof(Ingredient), nameof(quantity))
        );

        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }

        Quantity = quantity;

        ModifiedOnUtc = DateTime.UtcNow;

        return Result.Success();
    }
}
