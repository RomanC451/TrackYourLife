using FluentValidation.TestHelper;
using TrackYourLife.Modules.Nutrition.Application.Features.FoodDiaries.Commands.AddFoodDiary;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.FoodDiaries.Commands;

public class AddFoodDiaryCommandValidatorTests
{
    private readonly AddFoodDiaryCommandValidator validator = new();

    [Fact]
    public void Validator_WithValidaData_ShouldSucced()
    {
        // Arrange
        var command = new AddFoodDiaryCommand(
            FoodId.NewId(),
            MealTypes.Breakfast,
            ServingSizeId.NewId(),
            1,
            DateOnly.FromDateTime(DateTime.UtcNow)
        );

        // Act & Assert
        validator.TestValidate(command).ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validator_WithEmptyFoodId_ShouldFail()
    {
        // Arrange
        var command = new AddFoodDiaryCommand(
            FoodId.Empty,
            MealTypes.Breakfast,
            ServingSizeId.NewId(),
            1,
            DateOnly.FromDateTime(DateTime.UtcNow)
        );

        // Act & Assert
        validator.TestValidate(command).ShouldHaveValidationErrorFor(command => command.FoodId);
    }

    [Fact]
    public void Validator_WithInvalidMealType_ShouldFail()
    {
        // Arrange
        var command = new AddFoodDiaryCommand(
            FoodId.NewId(),
            (MealTypes)100,
            ServingSizeId.NewId(),
            1,
            DateOnly.FromDateTime(DateTime.UtcNow)
        );

        // Act & Assert
        validator.TestValidate(command).ShouldHaveValidationErrorFor(command => command.MealType);
    }

    [Fact]
    public void Validator_WithEmptyServingSizeId_ShouldFail()
    {
        // Arrange
        var command = new AddFoodDiaryCommand(
            FoodId.NewId(),
            MealTypes.Breakfast,
            ServingSizeId.Empty,
            1,
            DateOnly.FromDateTime(DateTime.UtcNow)
        );

        // Act & Assert
        validator
            .TestValidate(command)
            .ShouldHaveValidationErrorFor(command => command.ServingSizeId);
    }

    [Fact]
    public void Validator_WithZeroQuantity_ShouldFail()
    {
        // Arrange
        var command = new AddFoodDiaryCommand(
            FoodId.NewId(),
            MealTypes.Breakfast,
            ServingSizeId.NewId(),
            0,
            DateOnly.FromDateTime(DateTime.UtcNow)
        );

        // Act & Assert
        validator.TestValidate(command).ShouldHaveValidationErrorFor(command => command.Quantity);
    }

    [Fact]
    public void Validator_WithEmptyEntryDate_ShouldFail()
    {
        // Arrange
        var command = new AddFoodDiaryCommand(
            FoodId.NewId(),
            MealTypes.Breakfast,
            ServingSizeId.NewId(),
            1,
            DateOnly.MinValue
        );

        // Act & Assert
        validator.TestValidate(command).ShouldHaveValidationErrorFor(command => command.EntryDate);
    }
}
