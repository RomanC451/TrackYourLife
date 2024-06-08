using TrackYourLife.Common.Application.UserGoals.Commands.UpdateUserGoal;
using TrackYourLife.Common.Domain.UserGoals;

namespace TrackYourLife.Common.Application.UnitTests.UserGoals;

public class UpdateUserGoalCommandValidatorTests
{
    private readonly UpdateUserGoalCommandValidator _validator = new();

    [Fact]
    public void ShouldHaveErrorWhenIdIsDefault()
    {
        var command = new UpdateUserGoalCommand(
            Id: default!,
            Type: UserGoalType.Water,
            Value: 1,
            PerPeriod: UserGoalPerPeriod.Week,
            StartDate: DateOnly.FromDateTime(DateTime.Now),
            EndDate: DateOnly.FromDateTime(DateTime.Now).AddDays(1)
        );
        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Equal("Id", result.Errors[0].PropertyName);
    }

    [Fact]
    public void ShouldHaveErrorWhenValueIsZero()
    {
        var command = new UpdateUserGoalCommand(
            Id: UserGoalId.NewId(),
            Type: UserGoalType.Water,
            Value: 0,
            PerPeriod: UserGoalPerPeriod.Week,
            StartDate: DateOnly.FromDateTime(DateTime.Now),
            EndDate: DateOnly.FromDateTime(DateTime.Now).AddDays(1)
        );
        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Equal("Value", result.Errors[0].PropertyName);
    }

    [Fact]
    public void ShouldHaveErrorWhenStartDateIsDefault()
    {
        var command = new UpdateUserGoalCommand(
            Id: UserGoalId.NewId(),
            Type: UserGoalType.Water,
            Value: 1,
            PerPeriod: UserGoalPerPeriod.Week,
            StartDate: default,
            EndDate: DateOnly.FromDateTime(DateTime.Now).AddDays(1)
        );
        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Equal("StartDate", result.Errors[0].PropertyName);
    }

    [Fact]
    public void ShouldHaveErrorWhenEndDateIsLessThanStartDate()
    {
        var command = new UpdateUserGoalCommand(
            Id: UserGoalId.NewId(),
            Type: UserGoalType.Water,
            Value: 1,
            PerPeriod: UserGoalPerPeriod.Week,
            StartDate: DateOnly.FromDateTime(DateTime.Now),
            EndDate: DateOnly.FromDateTime(DateTime.Now).AddDays(-1)
        );
        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Equal("EndDate", result.Errors[0].PropertyName);
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
        var command = new UpdateUserGoalCommand(
            Id: UserGoalId.NewId(),
            Type: type,
            Value: value,
            PerPeriod: perPeriod,
            StartDate: DateOnly.Parse(startDate),
            EndDate: !string.IsNullOrEmpty(endDate) ? DateOnly.Parse(endDate) : null,
            Force: force
        );
        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }
}
