using FluentValidation.TestHelper;
using TrackYourLife.Modules.Nutrition.Application.Features.FoodDiaries.Commands.UpdateFoodDiary;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.FoodDiaries.Commands;

public class UpdateFoodDiaryCommandValidatorTests
{
    private readonly UpdateFoodDiaryCommandValidator validator = new();

    [Fact]
    public void Validator_WithValidaData_ShouldSucced()
    {
        // Arrange
        var command = new UpdateFoodDiaryCommand(
            NutritionDiaryId.NewId(),
            1,
            ServingSizeId.NewId(),
            MealTypes.Breakfast
        );

        // Act & Assert
        validator.TestValidate(command).ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validator_WithEmptyId_ShouldFail()
    {
        // Arrange
        var command = new UpdateFoodDiaryCommand(
            NutritionDiaryId.Empty,
            1,
            ServingSizeId.NewId(),
            MealTypes.Breakfast
        );

        // Act & Assert
        validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validator_WithInvalidQuantity_ShouldFail()
    {
        // Arrange
        var command = new UpdateFoodDiaryCommand(
            NutritionDiaryId.NewId(),
            0,
            ServingSizeId.NewId(),
            MealTypes.Breakfast
        );

        // Act & Assert
        validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Quantity);
    }

    [Fact]
    public void Validator_WithEmptyServingSizeId_ShouldFail()
    {
        // Arrange
        var command = new UpdateFoodDiaryCommand(
            NutritionDiaryId.NewId(),
            1,
            ServingSizeId.Empty,
            MealTypes.Breakfast
        );

        // Act & Assert
        validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.ServingSizeId);
    }

    [Fact]
    public void Validator_WithInvalidMealType_ShouldFail()
    {
        // Arrange
        var command = new UpdateFoodDiaryCommand(
            NutritionDiaryId.NewId(),
            1,
            ServingSizeId.NewId(),
            (MealTypes)100
        );

        // Act & Assert
        validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.MealType);
    }
}
