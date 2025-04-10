using FluentValidation.TestHelper;
using TrackYourLife.Modules.Nutrition.Application.Features.RecipeDiaries.Commands.AddRecipeDiary;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.Features.RecipeDiaries.Commands.AddRecipeDiary;

public class AddRecipeDiaryCommandValidatorTests
{
    private readonly AddRecipeDiaryCommandValidator _validator;

    public AddRecipeDiaryCommandValidatorTests()
    {
        _validator = new AddRecipeDiaryCommandValidator();
    }

    [Fact]
    public void Validate_WhenCommandIsValid_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new AddRecipeDiaryCommand(
            RecipeId.NewId(),
            MealTypes.Breakfast,
            1.0f,
            DateOnly.FromDateTime(DateTime.Now)
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
        var command = new AddRecipeDiaryCommand(
            RecipeId.Empty,
            MealTypes.Breakfast,
            1.0f,
            DateOnly.FromDateTime(DateTime.Now)
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.RecipeId);
    }

    [Fact]
    public void Validate_WhenMealTypeIsInvalid_ShouldHaveValidationError()
    {
        // Arrange
        var command = new AddRecipeDiaryCommand(
            RecipeId.NewId(),
            (MealTypes)999, // Invalid enum value
            1.0f,
            DateOnly.FromDateTime(DateTime.Now)
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MealType);
    }

    [Fact]
    public void Validate_WhenQuantityIsZero_ShouldHaveValidationError()
    {
        // Arrange
        var command = new AddRecipeDiaryCommand(
            RecipeId.NewId(),
            MealTypes.Breakfast,
            0,
            DateOnly.FromDateTime(DateTime.Now)
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
        var command = new AddRecipeDiaryCommand(
            RecipeId.NewId(),
            MealTypes.Breakfast,
            -1.0f,
            DateOnly.FromDateTime(DateTime.Now)
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Quantity);
    }

    [Fact]
    public void Validate_WhenEntryDateIsDefault_ShouldHaveValidationError()
    {
        // Arrange
        var command = new AddRecipeDiaryCommand(
            RecipeId.NewId(),
            MealTypes.Breakfast,
            1.0f,
            default
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.EntryDate);
    }
}
