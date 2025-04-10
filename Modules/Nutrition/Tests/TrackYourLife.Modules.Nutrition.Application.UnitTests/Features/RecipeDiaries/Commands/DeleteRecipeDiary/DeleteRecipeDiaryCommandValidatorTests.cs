using FluentValidation.TestHelper;
using TrackYourLife.Modules.Nutrition.Application.Features.RecipeDiaries.Commands.DeleteRecipeDiary;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.Features.RecipeDiaries.Commands.DeleteRecipeDiary;

public class DeleteRecipeDiaryCommandValidatorTests
{
    private readonly DeleteRecipeDiaryCommandValidator _validator;

    public DeleteRecipeDiaryCommandValidatorTests()
    {
        _validator = new DeleteRecipeDiaryCommandValidator();
    }

    [Fact]
    public void Validate_WhenIdIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new DeleteRecipeDiaryCommand(NutritionDiaryId.Empty);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validate_WhenIdIsValid_ShouldNotHaveValidationError()
    {
        // Arrange
        var command = new DeleteRecipeDiaryCommand(NutritionDiaryId.NewId());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
    }
}
