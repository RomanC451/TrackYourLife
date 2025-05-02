using FluentValidation.TestHelper;
using TrackYourLife.Modules.Users.Application.Features.Goals.Commands.UpdateNutritionGoals;

namespace TrackYourLife.Modules.Users.Application.UnitTests.Features.Goals.Commands.UpdateNutritionGoals;

public class UpdateNutritionGoalsCommandValidatorTests
{
    private readonly UpdateNutritionGoalsCommandValidator _validator;

    public UpdateNutritionGoalsCommandValidatorTests()
    {
        _validator = new UpdateNutritionGoalsCommandValidator();
    }

    [Fact]
    public void Validate_WithValidInput_ShouldBeValid()
    {
        // Arrange
        var command = new UpdateNutritionGoalsCommand(
            Calories: 2000,
            Protein: 150,
            Carbohydrates: 250,
            Fats: 70,
            Force: false
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithZeroCalories_ShouldBeInvalid()
    {
        // Arrange
        var command = new UpdateNutritionGoalsCommand(
            Calories: 0,
            Protein: 150,
            Carbohydrates: 250,
            Fats: 70,
            Force: false
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Calories);
    }

    [Fact]
    public void Validate_WithZeroProtein_ShouldBeInvalid()
    {
        // Arrange
        var command = new UpdateNutritionGoalsCommand(
            Calories: 2000,
            Protein: 0,
            Carbohydrates: 250,
            Fats: 70,
            Force: false
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Protein);
    }

    [Fact]
    public void Validate_WithZeroCarbohydrates_ShouldBeInvalid()
    {
        // Arrange
        var command = new UpdateNutritionGoalsCommand(
            Calories: 2000,
            Protein: 150,
            Carbohydrates: 0,
            Fats: 70,
            Force: false
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Carbohydrates);
    }

    [Fact]
    public void Validate_WithZeroFats_ShouldBeInvalid()
    {
        // Arrange
        var command = new UpdateNutritionGoalsCommand(
            Calories: 2000,
            Protein: 150,
            Carbohydrates: 250,
            Fats: 0,
            Force: false
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Fats);
    }
}
