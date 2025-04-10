using FluentValidation.TestHelper;
using TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Commands.AddIngredient;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.Ingredients;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.Features.Recipes.Commands.AddIngredient;

public class AddIngredientCommandValidatorTests
{
    private readonly AddIngredientCommandValidator _validator;

    public AddIngredientCommandValidatorTests()
    {
        _validator = new AddIngredientCommandValidator();
    }

    [Fact]
    public void Validate_WhenCommandIsValid_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new AddIngredientCommand(
            RecipeId: RecipeId.NewId(),
            FoodId: FoodId.NewId(),
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
        var command = new AddIngredientCommand(
            RecipeId: RecipeId.Empty,
            FoodId: FoodId.NewId(),
            ServingSizeId: ServingSizeId.NewId(),
            Quantity: 1.0f
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.RecipeId);
    }

    [Fact]
    public void Validate_WhenFoodIdIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new AddIngredientCommand(
            RecipeId: RecipeId.NewId(),
            FoodId: FoodId.Empty,
            ServingSizeId: ServingSizeId.NewId(),
            Quantity: 1.0f
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FoodId);
    }

    [Fact]
    public void Validate_WhenServingSizeIdIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new AddIngredientCommand(
            RecipeId: RecipeId.NewId(),
            FoodId: FoodId.NewId(),
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
        var command = new AddIngredientCommand(
            RecipeId: RecipeId.NewId(),
            FoodId: FoodId.NewId(),
            ServingSizeId: ServingSizeId.NewId(),
            Quantity: 0.0f
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Quantity);
    }

    [Fact]
    public void Validate_WhenQuantityIsNegative_ShouldHaveValidationError()
    {
        // Arrange
        var command = new AddIngredientCommand(
            RecipeId: RecipeId.NewId(),
            FoodId: FoodId.NewId(),
            ServingSizeId: ServingSizeId.NewId(),
            Quantity: -1.0f
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Quantity);
    }
}
