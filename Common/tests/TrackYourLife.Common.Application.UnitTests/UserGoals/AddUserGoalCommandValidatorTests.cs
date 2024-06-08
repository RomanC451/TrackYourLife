using TrackYourLife.Application.UserGoals.Commands.AddUserGoal;
using TrackYourLife.Common.Application.UserGoals.Commands.AddUserGoal;
using TrackYourLife.Common.Domain.UserGoals;

namespace TrackYourLife.Common.Application.UnitTests.UserGoals;

public sealed class AddUserGoalCommandValidatorTests
{
    private readonly AddUserGoalCommandValidator _validator = new();

    [Fact]
    public void ShouldHaveErrorWhenValueIsZero()
    {
        var command = new AddUserGoalCommand(
            Value: 0,
            Type: UserGoalType.Water,
            PerPeriod: UserGoalPerPeriod.Week,
            StartDate: DateOnly.FromDateTime(DateTime.Now),
            EndDate: DateOnly.FromDateTime(DateTime.Now).AddDays(1),
            Force: false
        );
        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Equal("Value", result.Errors[0].PropertyName);
    }

    [Fact]
    public void ShouldHaveErrorWhenStartDateIsDefault()
    {
        var command = new AddUserGoalCommand(
            Value: 1,
            Type: UserGoalType.Water,
            PerPeriod: UserGoalPerPeriod.Week,
            StartDate: default,
            EndDate: DateOnly.FromDateTime(DateTime.Now).AddDays(1),
            Force: false
        );
        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Equal("StartDate", result.Errors[0].PropertyName);
    }

    [Fact]
    public void ShouldHaveErrorWhenEndDateIsLessThanStartDate()
    {
        var command = new AddUserGoalCommand(
            Value: 1,
            Type: UserGoalType.Water,
            PerPeriod: UserGoalPerPeriod.Week,
            StartDate: DateOnly.FromDateTime(DateTime.Now),
            EndDate: DateOnly.FromDateTime(DateTime.Now).AddDays(-1),
            Force: false
        );
        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Equal("EndDate", result.Errors[0].PropertyName);
    }

    [Fact]
    public void ShouldNotHaveErrorWhenEndDateIsNull()
    {
        var command = new AddUserGoalCommand(
            Value: 1,
            Type: UserGoalType.Water,
            PerPeriod: UserGoalPerPeriod.Week,
            StartDate: DateOnly.FromDateTime(DateTime.Now),
            EndDate: default,
            Force: false
        );
        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(300, UserGoalType.Water, UserGoalPerPeriod.Week, "2022-01-01", "2022-01-02", false)]
    [InlineData(5600, UserGoalType.Calories, UserGoalPerPeriod.Month, "2022-01-01", null, true)]
    [InlineData(
        12,
        UserGoalType.Calories,
        UserGoalPerPeriod.Day,
        "2022-01-01",
        "2022-01-01",
        false
    )]
    public void ShouldNotHaveErrorWithValidArguments(
        int value,
        UserGoalType type,
        UserGoalPerPeriod perPeriod,
        string startDate,
        string endDate,
        bool force
    )
    {
        var command = new AddUserGoalCommand(
            Value: value,
            Type: type,
            PerPeriod: perPeriod,
            StartDate: DateOnly.Parse(startDate),
            EndDate: !string.IsNullOrEmpty(endDate) ? DateOnly.Parse(endDate) : null,
            Force: force
        );
        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }
}
