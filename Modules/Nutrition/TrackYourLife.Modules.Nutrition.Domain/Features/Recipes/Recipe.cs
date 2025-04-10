using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.Ingredients;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Domain.Utils;

namespace TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;

public sealed class Recipe : Entity<RecipeId>, IAuditableEntity
{
    public UserId UserId { get; private set; } = UserId.Empty;
    public string Name { get; private set; } = string.Empty;
    public List<Ingredient> Ingredients { get; private set; } = [];
    public NutritionalContent NutritionalContents { get; private set; } = new();
    public int Portions { get; private set; } = 1;
    public bool IsOld { get; private set; } = false;
    public DateTime CreatedOnUtc { get; private set; } = DateTime.UtcNow;
    public DateTime? ModifiedOnUtc { get; private set; } = null;

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    [ConcurrencyCheck]
    public uint Xmin { get; private set; }

    private Recipe()
        : base() { }

    private Recipe(
        RecipeId id,
        UserId userId,
        string name,
        int portions = 1,
        List<Ingredient>? ingredients = null,
        NutritionalContent? nutritionalContents = null,
        DateTime? createdOnUtc = null
    )
        : base(id)
    {
        UserId = userId;
        Name = name;
        Ingredients = ingredients ?? [];
        NutritionalContents = nutritionalContents ?? new();
        Portions = portions;
        CreatedOnUtc = createdOnUtc ?? DateTime.UtcNow;
    }

    public static Result<Recipe> Create(RecipeId id, UserId userId, string name)
    {
        var result = Result.FirstFailureOrSuccess(
            Ensure.NotEmptyId(id, DomainErrors.ArgumentError.Empty(nameof(Recipe), nameof(id))),
            Ensure.NotEmptyId(
                userId,
                DomainErrors.ArgumentError.Empty(nameof(Recipe), nameof(userId))
            ),
            Ensure.NotEmpty(name, DomainErrors.ArgumentError.Empty(nameof(Recipe), nameof(name)))
        );

        if (result.IsFailure)
        {
            return Result.Failure<Recipe>(result.Error);
        }

        return Result.Success(new Recipe(id, userId, name));
    }

    public Result<Recipe> Clone(RecipeId id)
    {
        var result = Result.FirstFailureOrSuccess(
            Ensure.NotEmptyId(id, DomainErrors.ArgumentError.Empty(nameof(Recipe), nameof(id)))
        );

        if (result.IsFailure)
        {
            return Result.Failure<Recipe>(result.Error);
        }

        var clonedIngredientsResults = Ingredients
            .Select(i => i.Clone(IngredientId.NewId()))
            .ToList();

        if (clonedIngredientsResults.Exists(r => r.IsFailure))
        {
            return Result.Failure<Recipe>(clonedIngredientsResults.First(r => r.IsFailure).Error);
        }

        var clonedIngredients = clonedIngredientsResults.Select(r => r.Value).ToList();

        var clone = new Recipe(id, UserId, Name, Portions, clonedIngredients, NutritionalContents);

        return Result.Success(clone);
    }

    public Result AddIngredient(Ingredient ingredient, Food food, ServingSizeReadModel servingSize)
    {
        var result = Result.FirstFailureOrSuccess(
            Ensure.IsFalse(IsOld, RecipeErrors.Old),
            Ensure.NotNull(
                ingredient,
                DomainErrors.ArgumentError.Null(nameof(Recipe), nameof(ingredient))
            )
        );

        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }

        var existingIngredient = Ingredients.Find(i => i.FoodId == ingredient.FoodId);

        ModifiedOnUtc = DateTime.UtcNow;

        if (existingIngredient is null)
        {
            Ingredients.Add(ingredient);

            NutritionalContents.AddNutritionalValues(
                food.NutritionalContents.MultiplyNutritionalValues(
                    servingSize.NutritionMultiplier * ingredient.Quantity
                )
            );

            return Result.Success();
        }

        if (existingIngredient.ServingSizeId == ingredient.ServingSizeId)
        {
            existingIngredient.UpdateQuantity(existingIngredient.Quantity + ingredient.Quantity);

            NutritionalContents.AddNutritionalValues(
                food.NutritionalContents.MultiplyNutritionalValues(
                    servingSize.NutritionMultiplier * ingredient.Quantity
                )
            );

            return Result.Success();
        }

        return Result.Failure(IngredientErrors.Duplicate(ingredient.FoodId));
    }

    public Result RemoveIngredient(
        Ingredient ingredient,
        Food food,
        ServingSizeReadModel servingSize
    )
    {
        var result = Result.FirstFailureOrSuccess(
            Ensure.IsFalse(IsOld, RecipeErrors.Old),
            Ensure.NotNull(
                ingredient,
                DomainErrors.ArgumentError.Null(nameof(Recipe), nameof(ingredient))
            )
        );

        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }

        ModifiedOnUtc = DateTime.UtcNow;

        Ingredients.Remove(ingredient);

        NutritionalContents.SubtractNutritionalValues(
            food.NutritionalContents.MultiplyNutritionalValues(
                servingSize.NutritionMultiplier * ingredient.Quantity
            )
        );

        return Result.Success();
    }

    public Result UpdateIngredient(
        Food food,
        Ingredient ingredient,
        ServingSize oldServingSize,
        ServingSize newServingSize,
        float oldQuantity
    )
    {
        var result = Result.FirstFailureOrSuccess(
            Ensure.IsFalse(IsOld, RecipeErrors.Old),
            Ensure.NotNull(
                ingredient,
                DomainErrors.ArgumentError.Null(nameof(Recipe), nameof(ingredient))
            )
        );

        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }

        var existingIngredient = Ingredients.Find(i => i.Id == ingredient.Id);

        if (existingIngredient is null)
        {
            return Result.Failure(IngredientErrors.NotFound(ingredient.Id));
        }

        Ingredients.Remove(existingIngredient);
        Ingredients.Add(ingredient);

        NutritionalContents.SubtractNutritionalValues(
            food.NutritionalContents.MultiplyNutritionalValues(
                oldServingSize.NutritionMultiplier * oldQuantity
            )
        );

        NutritionalContents.AddNutritionalValues(
            food.NutritionalContents.MultiplyNutritionalValues(
                newServingSize.NutritionMultiplier * ingredient.Quantity
            )
        );

        ModifiedOnUtc = DateTime.UtcNow;

        return Result.Success();
    }

    public Result UpdatePortions(int portions)
    {
        var result = Result.FirstFailureOrSuccess(
            Ensure.IsFalse(IsOld, RecipeErrors.Old),
            Ensure.NotNegative(
                portions,
                DomainErrors.ArgumentError.Negative(nameof(Recipe), nameof(portions))
            )
        );

        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }

        Portions = portions;

        ModifiedOnUtc = DateTime.UtcNow;

        return Result.Success();
    }

    public Result UpdateName(string name)
    {
        var result = Result.FirstFailureOrSuccess(
            Ensure.IsFalse(IsOld, RecipeErrors.Old),
            Ensure.NotEmpty(name, DomainErrors.ArgumentError.Empty(nameof(Recipe), nameof(name)))
        );

        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }

        Name = name;

        ModifiedOnUtc = DateTime.UtcNow;

        return Result.Success();
    }

    public Ingredient? GetIngredientById(IngredientId ingredientId)
    {
        return Ingredients.Find(i => i.Id == ingredientId);
    }

    public Ingredient? GetIngredientByFoodId(FoodId foodId)
    {
        return Ingredients.Find(i => i.FoodId == foodId);
    }

    public void MarkAsOld()
    {
        IsOld = true;
        ModifiedOnUtc = DateTime.UtcNow;
    }

    public void RemoveOldMark()
    {
        IsOld = false;
        ModifiedOnUtc = DateTime.UtcNow;
    }
}
