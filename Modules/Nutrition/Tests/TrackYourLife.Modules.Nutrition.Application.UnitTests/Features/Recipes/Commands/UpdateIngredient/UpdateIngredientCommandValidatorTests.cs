using FluentValidation.TestHelper;
using TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Commands.UpdateIngredient;
using TrackYourLife.Modules.Nutrition.Domain.Features.Ingredients;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.Features.Recipes.Commands.UpdateIngredient;

public class UpdateIngredientCommandValidatorTests
{
    private readonly UpdateIngredientCommandValidator _validator;

    public UpdateIngredientCommandValidatorTests()
    {
        _validator = new UpdateIngredientCommandValidator();
    }

    [Fact]
    public void Validate_WhenCommandIsValid_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new UpdateIngredientCommand(
            RecipeId: RecipeId.NewId(),
            IngredientId: IngredientId.NewId(),
            ServingSizeId: ServingSizeId.NewId(),
            Quantity: 1.0f
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
        var command = new UpdateIngredientCommand(
            RecipeId: RecipeId.Empty,
            IngredientId: IngredientId.NewId(),
            ServingSizeId: ServingSizeId.NewId(),
            Quantity: 1.0f
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.RecipeId);
    }

    [Fact]
    public void Validate_WhenIngredientIdIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new UpdateIngredientCommand(
            RecipeId: RecipeId.NewId(),
            IngredientId: IngredientId.Empty,
            ServingSizeId: ServingSizeId.NewId(),
            Quantity: 1.0f
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.IngredientId);
    }

    [Fact]
    public void Validate_WhenServingSizeIdIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new UpdateIngredientCommand(
            RecipeId: RecipeId.NewId(),
            IngredientId: IngredientId.NewId(),
            ServingSizeId: ServingSizeId.Empty,
            Quantity: 1.0f
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ServingSizeId);
    }

    [Fact]
    public void Validate_WhenQuantityIsZero_ShouldHaveValidationError()
    {
        // Arrange
        var command = new UpdateIngredientCommand(
            RecipeId: RecipeId.NewId(),
            IngredientId: IngredientId.NewId(),
            ServingSizeId: ServingSizeId.NewId(),
            Quantity: 0.0f
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Quantity);
    }
}
