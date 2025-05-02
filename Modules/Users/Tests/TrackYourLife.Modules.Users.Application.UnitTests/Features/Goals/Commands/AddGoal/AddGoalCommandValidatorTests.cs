using FluentValidation.TestHelper;
using TrackYourLife.Modules.Users.Application.Features.Goals.Commands.AddGoal;
using TrackYourLife.Modules.Users.Domain.Features.Goals;
using TrackYourLife.SharedLib.Domain.Enums;

namespace TrackYourLife.Modules.Users.Application.UnitTests.Features.Goals.Commands.AddGoal;

public class AddGoalCommandValidatorTests
{
    private readonly AddGoalCommandValidator _validator;

    public AddGoalCommandValidatorTests()
    {
        _validator = new AddGoalCommandValidator();
    }

    [Fact]
    public void Validate_WithValidInput_ShouldBeValid()
    {
        // Arrange
        var command = new AddGoalCommand(
            Value: 100,
            Type: GoalType.Calories,
            PerPeriod: GoalPeriod.Day,
            StartDate: DateOnly.FromDateTime(DateTime.UtcNow),
            EndDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)),
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
    public void Validate_WithInvalidValue_ShouldBeInvalid(int value)
    {
        // Arrange
        var command = new AddGoalCommand(
            Value: value,
            Type: GoalType.Calories,
            PerPeriod: GoalPeriod.Day,
            StartDate: DateOnly.FromDateTime(DateTime.UtcNow),
            EndDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)),
            Force: false
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Value);
    }

    [Fact]
    public void Validate_WithInvalidType_ShouldBeInvalid()
    {
        // Arrange
        var command = new AddGoalCommand(
            Value: 100,
            Type: (GoalType)999, // Invalid enum value
            PerPeriod: GoalPeriod.Day,
            StartDate: DateOnly.FromDateTime(DateTime.UtcNow),
            EndDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)),
            Force: false
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Type).WithErrorMessage("Invalid goal type.");
    }

    [Fact]
    public void Validate_WithInvalidPerPeriod_ShouldBeInvalid()
    {
        // Arrange
        var command = new AddGoalCommand(
            Value: 100,
            Type: GoalType.Calories,
            PerPeriod: (GoalPeriod)999, // Invalid enum value
            StartDate: DateOnly.FromDateTime(DateTime.UtcNow),
            EndDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)),
            Force: false
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result
            .ShouldHaveValidationErrorFor(x => x.PerPeriod)
            .WithErrorMessage("Invalid goal period.");
    }

    [Fact]
    public void Validate_WithEndDateBeforeStartDate_ShouldBeInvalid()
    {
        // Arrange
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var command = new AddGoalCommand(
            Value: 100,
            Type: GoalType.Calories,
            PerPeriod: GoalPeriod.Day,
            StartDate: startDate,
            EndDate: startDate.AddDays(-1),
            Force: false
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.EndDate);
    }
}
