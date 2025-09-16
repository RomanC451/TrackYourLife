using FluentValidation.TestHelper;
using TrackYourLife.Modules.Nutrition.Application.Features.RecipeDiaries.Commands.UpdateRecipeDiary;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;

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
            MealTypes.Breakfast,
            ServingSizeId.NewId(),
            DateOnly.FromDateTime(DateTime.Now)
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
            MealTypes.Breakfast,
            ServingSizeId.NewId(),
            DateOnly.FromDateTime(DateTime.Now)
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
            MealTypes.Breakfast,
            ServingSizeId.NewId(),
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
        var command = new UpdateRecipeDiaryCommand(
            NutritionDiaryId.NewId(),
            -1.0f,
            MealTypes.Breakfast,
            ServingSizeId.NewId(),
            DateOnly.FromDateTime(DateTime.Now)
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
            (MealTypes)999,
            ServingSizeId.NewId(),
            DateOnly.FromDateTime(DateTime.Now)
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MealType);
    }
}
