using TrackYourLife.Application.UserGoals.Commands.RemoveUserGoal;
using TrackYourLife.Common.Application.UserGoals.Commands.RemoveUserGoal;
using TrackYourLife.Common.Domain.UserGoals;

namespace TrackYourLife.Common.Application.UnitTests.UserGoals;

public class RemoveUserGoalCommandValidatorTests
{
    private readonly RemoveUserGoalCommandValidator _validator = new();

    [Fact]
    public void ShouldHaveErrorWhenIdIsDefault()
    {
        var command = new RemoveUserGoalCommand(default!);
        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Equal("Id", result.Errors[0].PropertyName);
    }

    [Fact]
    public void ShouldNotHaveErrorWhenIdIsNotEmpty()
    {
        var command = new RemoveUserGoalCommand(UserGoalId.NewId());
        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }
}
