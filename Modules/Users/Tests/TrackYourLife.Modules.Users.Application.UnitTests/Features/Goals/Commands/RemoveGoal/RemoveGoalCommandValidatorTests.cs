using FluentValidation.TestHelper;
using TrackYourLife.Modules.Users.Application.Features.Goals.Commands.RemoveGoal;
using TrackYourLife.Modules.Users.Domain.Features.Goals;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Users.Application.UnitTests.Features.Goals.Commands.RemoveGoal;

public class RemoveGoalCommandValidatorTests
{
    private readonly RemoveGoalCommandValidator _validator;

    public RemoveGoalCommandValidatorTests()
    {
        _validator = new RemoveGoalCommandValidator();
    }

    [Fact]
    public void Validate_WithValidInput_ShouldBeValid()
    {
        // Arrange
        var command = new RemoveGoalCommand(GoalId.NewId());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyId_ShouldBeInvalid()
    {
        // Arrange
        var command = new RemoveGoalCommand(GoalId.Empty);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}
