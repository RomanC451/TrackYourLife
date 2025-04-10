using FluentValidation.TestHelper;
using TrackYourLife.Modules.Nutrition.Application.Features.FoodDiaries.Commands.UpdateFoodDiary;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.Features.FoodDiaries.Commands.UpdateFoodDiary;

public class UpdateFoodDiaryCommandValidatorTests
{
    private readonly UpdateFoodDiaryCommandValidator _validator;
    private readonly UpdateFoodDiaryCommand _validCommand;

    public UpdateFoodDiaryCommandValidatorTests()
    {
        _validator = new UpdateFoodDiaryCommandValidator();
        _validCommand = new UpdateFoodDiaryCommand(
            NutritionDiaryId.NewId(),
            1.5f,
            ServingSizeId.NewId(),
            MealTypes.Breakfast
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
    public void Validate_WhenIdIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = _validCommand with
        {
            Id = NutritionDiaryId.Empty,
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
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
}
