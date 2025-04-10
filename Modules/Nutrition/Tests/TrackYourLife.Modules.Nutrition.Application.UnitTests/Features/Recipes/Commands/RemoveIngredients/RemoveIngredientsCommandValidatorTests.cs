using FluentValidation.TestHelper;
using TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Commands.RemoveIngredients;
using TrackYourLife.Modules.Nutrition.Domain.Features.Ingredients;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.Features.Recipes.Commands.RemoveIngredients;

public class RemoveIngredientsCommandValidatorTests
{
    private readonly RemoveIngredientsCommandValidator _validator;

    public RemoveIngredientsCommandValidatorTests()
    {
        _validator = new RemoveIngredientsCommandValidator();
    }

    [Fact]
    public void Validate_WhenCommandIsValid_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new RemoveIngredientsCommand(
            RecipeId: RecipeId.NewId(),
            IngredientsIds: [IngredientId.NewId(), IngredientId.NewId()]
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenRecipeIdIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new RemoveIngredientsCommand(
            RecipeId: RecipeId.Empty,
            IngredientsIds: [IngredientId.NewId()]
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.RecipeId);
    }

    [Fact]
    public void Validate_WhenIngredientsIdsContainEmptyId_ShouldHaveValidationError()
    {
        // Arrange
        var command = new RemoveIngredientsCommand(
            RecipeId: RecipeId.NewId(),
            IngredientsIds: [IngredientId.Empty, IngredientId.NewId()]
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.IngredientsIds);
    }

    [Fact]
    public void Validate_WhenIngredientsIdsAreEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new RemoveIngredientsCommand(RecipeId: RecipeId.NewId(), IngredientsIds: []);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.IngredientsIds);
    }
}
