using FluentValidation.TestHelper;
using TrackYourLife.Modules.Nutrition.Application.Features.RecipeDiaries.Commands.UpdateRecipeDiary;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.Features.RecipeDiaries.Commands.UpdateRecipeDiary;

public class UpdateRecipeDiaryCommandValidatorTests
{
    private readonly UpdateRecipeDiaryCommandValidator _validator;

    public UpdateRecipeDiaryCommandValidatorTests()
    {
        _validator = new UpdateRecipeDiaryCommandValidator();
    }

    [Fact]
    public void Validate_WhenCommandIsValid_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new UpdateRecipeDiaryCommand(
            NutritionDiaryId.NewId(),
            1.0f,
            MealTypes.Breakfast
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenIdIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new UpdateRecipeDiaryCommand(
            NutritionDiaryId.Empty,
            1.0f,
            MealTypes.Breakfast
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validate_WhenQuantityIsZero_ShouldHaveValidationError()
    {
        // Arrange
        var command = new UpdateRecipeDiaryCommand(
            NutritionDiaryId.NewId(),
            0,
            MealTypes.Breakfast
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
        var command = new UpdateRecipeDiaryCommand(
            NutritionDiaryId.NewId(),
            -1.0f,
            MealTypes.Breakfast
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Quantity);
    }

    [Fact]
    public void Validate_WhenMealTypeIsInvalid_ShouldHaveValidationError()
    {
        // Arrange
        var command = new UpdateRecipeDiaryCommand(
            NutritionDiaryId.NewId(),
            1.0f,
            (MealTypes)999 // Invalid enum value
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MealType);
    }
}
