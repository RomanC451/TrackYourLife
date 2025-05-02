using FluentValidation.TestHelper;
using TrackYourLife.Modules.Users.Application.Features.Goals.Commands.UpdateGoal;
using TrackYourLife.Modules.Users.Domain.Features.Goals;
using TrackYourLife.SharedLib.Domain.Enums;

namespace TrackYourLife.Modules.Users.Application.UnitTests.Features.Goals.Commands.UpdateGoal;

public class UpdateGoalCommandValidatorTests
{
    private readonly UpdateGoalCommandValidator _validator;

    public UpdateGoalCommandValidatorTests()
    {
        _validator = new UpdateGoalCommandValidator();
    }

    [Fact]
    public void Validate_WithValidInput_ShouldBeValid()
    {
        // Arrange
        var command = new UpdateGoalCommand(
            GoalId.NewId(),
            GoalType.Calories,
            10000,
            GoalPeriod.Day,
            DateOnly.FromDateTime(DateTime.UtcNow),
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30))
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyId_ShouldBeInvalid()
    {
        // Arrange
        var command = new UpdateGoalCommand(
            GoalId.Empty,
            GoalType.Calories,
            10000,
            GoalPeriod.Day,
            DateOnly.FromDateTime(DateTime.UtcNow),
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30))
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validate_WithNegativeValue_ShouldBeInvalid()
    {
        // Arrange
        var command = new UpdateGoalCommand(
            GoalId.NewId(),
            GoalType.Calories,
            -10000,
            GoalPeriod.Day,
            DateOnly.FromDateTime(DateTime.UtcNow),
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30))
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Value);
    }

    [Fact]
    public void Validate_WithEndDateBeforeStartDate_ShouldBeInvalid()
    {
        // Arrange
        var command = new UpdateGoalCommand(
            GoalId.NewId(),
            GoalType.Calories,
            10000,
            GoalPeriod.Day,
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)),
            DateOnly.FromDateTime(DateTime.UtcNow)
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.EndDate);
    }
}
