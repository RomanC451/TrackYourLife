using FluentValidation.TestHelper;
using TrackYourLife.Modules.Nutrition.Application.Features.FoodDiaries.Commands.DeleteFoodDiary;
using TrackYourLife.Modules.Nutrition.Application.Features.RecipeDiaries.Commands.DeleteRecipeDiary;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.FoodDiaries.Commands;

public class RemoveFoodDiaryCommandValidatorTests
{
    private readonly DeleteFoodDiaryCommandValidator validator = new();

    [Fact]
    public void Validator_WithValidaData_ShouldSucced()
    {
        // Arrange
        var command = new RemoveFoodDiaryCommand(NutritionDiaryId.NewId());

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validator_WithEmptyId_ShouldFail()
    {
        // Arrange
        var command = new RemoveFoodDiaryCommand(NutritionDiaryId.Empty);

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}
