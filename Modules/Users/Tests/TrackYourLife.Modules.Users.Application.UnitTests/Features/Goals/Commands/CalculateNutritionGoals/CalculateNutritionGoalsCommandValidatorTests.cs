using FluentValidation.TestHelper;
using TrackYourLife.Modules.Users.Application.Features.Goals.Commands.CalculateNutritionGoals;
using TrackYourLife.Modules.Users.Domain.Features.Goals.Enums;
using TrackYourLife.SharedLib.Domain.Enums;

namespace TrackYourLife.Modules.Users.Application.UnitTests.Features.Goals.Commands.CalculateNutritionGoals;

public class CalculateNutritionGoalsCommandValidatorTests
{
    private readonly CalculateNutritionGoalsCommandValidator _validator;

    public CalculateNutritionGoalsCommandValidatorTests()
    {
        _validator = new CalculateNutritionGoalsCommandValidator();
    }

    [Fact]
    public void Validate_WithValidInput_ShouldBeValid()
    {
        // Arrange
        var command = new CalculateNutritionGoalsCommand(
            Age: 25,
            Weight: 70,
            Height: 180,
            Gender: Gender.Male,
            ActivityLevel: ActivityLevel.ModeratelyActive,
            FitnessGoal: FitnessGoal.Maintain,
            Force: false
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WithInvalidAge_ShouldBeInvalid(int age)
    {
        // Arrange
        var command = new CalculateNutritionGoalsCommand(
            Age: age,
            Weight: 70,
            Height: 180,
            Gender: Gender.Male,
            ActivityLevel: ActivityLevel.ModeratelyActive,
            FitnessGoal: FitnessGoal.Maintain,
            Force: false
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Age);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WithInvalidWeight_ShouldBeInvalid(int weight)
    {
        // Arrange
        var command = new CalculateNutritionGoalsCommand(
            Age: 25,
            Weight: weight,
            Height: 180,
            Gender: Gender.Male,
            ActivityLevel: ActivityLevel.ModeratelyActive,
            FitnessGoal: FitnessGoal.Maintain,
            Force: false
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Weight);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WithInvalidHeight_ShouldBeInvalid(int height)
    {
        // Arrange
        var command = new CalculateNutritionGoalsCommand(
            Age: 25,
            Weight: 70,
            Height: height,
            Gender: Gender.Male,
            ActivityLevel: ActivityLevel.ModeratelyActive,
            FitnessGoal: FitnessGoal.Maintain,
            Force: false
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Height);
    }

    [Fact]
    public void Validate_WithInvalidGender_ShouldBeInvalid()
    {
        // Arrange
        var command = new CalculateNutritionGoalsCommand(
            Age: 25,
            Weight: 70,
            Height: 180,
            Gender: (Gender)999, // Invalid enum value
            ActivityLevel: ActivityLevel.ModeratelyActive,
            FitnessGoal: FitnessGoal.Maintain,
            Force: false
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Gender);
    }

    [Fact]
    public void Validate_WithInvalidActivityLevel_ShouldBeInvalid()
    {
        // Arrange
        var command = new CalculateNutritionGoalsCommand(
            Age: 25,
            Weight: 70,
            Height: 180,
            Gender: Gender.Male,
            ActivityLevel: (ActivityLevel)999, // Invalid enum value
            FitnessGoal: FitnessGoal.Maintain,
            Force: false
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ActivityLevel);
    }

    [Fact]
    public void Validate_WithInvalidFitnessGoal_ShouldBeInvalid()
    {
        // Arrange
        var command = new CalculateNutritionGoalsCommand(
            Age: 25,
            Weight: 70,
            Height: 180,
            Gender: Gender.Male,
            ActivityLevel: ActivityLevel.ModeratelyActive,
            FitnessGoal: (FitnessGoal)999, // Invalid enum value
            Force: false
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FitnessGoal);
    }
}
