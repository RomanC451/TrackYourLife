using FluentValidation.TestHelper;
using TrackYourLife.Modules.Nutrition.Application.Features.FoodDiaries.Commands.AddFoodDiary;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.Features.FoodDiaries.Commands.AddFoodDiary;

public class AddFoodDiaryCommandValidatorTests
{
    private readonly AddFoodDiaryCommandValidator _validator;
    private readonly AddFoodDiaryCommand _validCommand;

    public AddFoodDiaryCommandValidatorTests()
    {
        _validator = new AddFoodDiaryCommandValidator();
        _validCommand = new AddFoodDiaryCommand(
            FoodId.NewId(),
            MealTypes.Breakfast,
            ServingSizeId.NewId(),
            1.5f,
            DateOnly.FromDateTime(DateTime.Today)
        );
    }

    [Fact]
    public void Validate_WhenAllPropertiesAreValid_ShouldNotHaveValidationErrors()
    {
        // Act
        var result = _validator.TestValidate(_validCommand);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenFoodIdIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = _validCommand with
        {
            FoodId = FoodId.Empty,
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FoodId);
    }

    [Fact]
    public void Validate_WhenMealTypeIsInvalid_ShouldHaveValidationError()
    {
        // Arrange
        var command = _validCommand with
        {
            MealType = (MealTypes)999,
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MealType);
    }

    [Fact]
    public void Validate_WhenServingSizeIdIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = _validCommand with
        {
            ServingSizeId = ServingSizeId.Empty,
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ServingSizeId);
    }

    [Fact]
    public void Validate_WhenQuantityIsZero_ShouldHaveValidationError()
    {
        // Arrange
        var command = _validCommand with
        {
            Quantity = 0,
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Quantity);
    }

    [Fact]
    public void Validate_WhenQuantityIsNegative_ShouldHaveValidationError()
    {
        // Arrange
        var command = _validCommand with
        {
            Quantity = -1.5f,
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Quantity);
    }

    [Fact]
    public void Validate_WhenEntryDateIsDefault_ShouldHaveValidationError()
    {
        // Arrange
        var command = _validCommand with
        {
            EntryDate = default,
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.EntryDate);
    }
}
