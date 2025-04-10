using FluentValidation.TestHelper;
using TrackYourLife.Modules.Nutrition.Application.Features.FoodDiaries.Commands.DeleteFoodDiary;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.Features.FoodDiaries.Commands.DeleteFoodDiary;

public class DeleteFoodDiaryCommandValidatorTests
{
    private readonly DeleteFoodDiaryCommandValidator _validator;
    private readonly DeleteFoodDiaryCommand _validCommand;

    public DeleteFoodDiaryCommandValidatorTests()
    {
        _validator = new DeleteFoodDiaryCommandValidator();
        _validCommand = new DeleteFoodDiaryCommand(NutritionDiaryId.NewId());
    }

    [Fact]
    public void Validate_WhenIdIsValid_ShouldNotHaveValidationErrors()
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
}
