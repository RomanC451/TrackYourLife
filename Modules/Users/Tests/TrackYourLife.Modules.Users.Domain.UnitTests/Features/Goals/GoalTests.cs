using FluentAssertions;
using TrackYourLife.Modules.Users.Domain.Features.Goals;
using TrackYourLife.Modules.Users.Domain.UnitTests.Utils;
using TrackYourLife.SharedLib.Domain.Enums;
using TrackYourLife.SharedLib.Domain.Ids;
using Xunit;

namespace TrackYourLife.Modules.Users.Domain.UnitTests.Features.Goals;

public class GoalTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldCreateGoal()
    {
        // Arrange
        var id = GoalId.NewId();
        var userId = UserId.NewId();
        var type = GoalType.Calories;
        var value = 10000;
        var period = GoalPeriod.Day;
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var endDate = DateOnly.MaxValue;

        // Act
        var result = Goal.Create(id, userId, type, value, period, startDate, endDate);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(id);
        result.Value.UserId.Should().Be(userId);
        result.Value.Type.Should().Be(type);
        result.Value.Value.Should().Be(value);
        result.Value.Period.Should().Be(period);
        result.Value.StartDate.Should().Be(startDate);
        result.Value.EndDate.Should().Be(endDate);
    }

    [Fact]
    public void Create_WithStartDateAfterEndDate_ShouldFail()
    {
        // Arrange
        var id = GoalId.NewId();
        var userId = UserId.NewId();
        var type = GoalType.Calories;
        var value = 10000;
        var period = GoalPeriod.Day;
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var endDate = startDate.AddDays(-1);

        // Act
        var result = Goal.Create(id, userId, type, value, period, startDate, endDate);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void UpdateType_WithValidType_ShouldUpdateType()
    {
        // Arrange
        var goal = GoalFaker.Generate();
        var newType = GoalType.Calories;

        // Act
        var result = goal.UpdateType(newType);

        // Assert
        result.IsSuccess.Should().BeTrue();
        goal.Type.Should().Be(newType);
    }

    [Fact]
    public void UpdateValue_WithValidValue_ShouldUpdateValue()
    {
        // Arrange
        var goal = GoalFaker.Generate();
        var newValue = 20000;

        // Act
        var result = goal.UpdateValue(newValue);

        // Assert
        result.IsSuccess.Should().BeTrue();
        goal.Value.Should().Be(newValue);
    }

    [Fact]
    public void UpdateValue_WithZeroValue_ShouldFail()
    {
        // Arrange
        var goal = GoalFaker.Generate();
        var newValue = 0;

        // Act
        var result = goal.UpdateValue(newValue);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void UpdatePerPeriod_WithValidPeriod_ShouldUpdatePeriod()
    {
        // Arrange
        var goal = GoalFaker.Generate();
        var newPeriod = GoalPeriod.Day;

        // Act
        var result = goal.UpdatePerPeriod(newPeriod);

        // Assert
        result.IsSuccess.Should().BeTrue();
        goal.Period.Should().Be(newPeriod);
    }

    [Fact]
    public void UpdateStartDate_WithValidDate_ShouldUpdateStartDate()
    {
        // Arrange
        var goal = GoalFaker.Generate();
        var newStartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));

        // Act
        var result = goal.UpdateStartDate(newStartDate);

        // Assert
        result.IsSuccess.Should().BeTrue();
        goal.StartDate.Should().Be(newStartDate);
    }

    [Fact]
    public void UpdateStartDate_WithDateAfterEndDate_ShouldFail()
    {
        // Arrange
        var goal = GoalFaker.Generate(
            startDate: DateOnly.FromDateTime(DateTime.UtcNow),
            endDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30))
        );
        var newStartDate = goal.EndDate.AddDays(1);

        // Act
        var result = goal.UpdateStartDate(newStartDate);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void UpdateEndDate_WithValidDate_ShouldUpdateEndDate()
    {
        // Arrange
        var goal = GoalFaker.Generate();
        var newEndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30));

        // Act
        var result = goal.UpdateEndDate(newEndDate);

        // Assert
        result.IsSuccess.Should().BeTrue();
        goal.EndDate.Should().Be(newEndDate);
    }

    [Fact]
    public void UpdateEndDate_WithDateBeforeStartDate_ShouldFail()
    {
        // Arrange
        var goal = GoalFaker.Generate();
        var newEndDate = goal.StartDate.AddDays(-1);

        // Act
        var result = goal.UpdateEndDate(newEndDate);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void IsActive_WhenEndDateIsMaxValue_ShouldReturnTrue()
    {
        // Arrange
        var goal = GoalFaker.Generate(endDate: DateOnly.MaxValue);

        // Act
        var result = goal.IsActive();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsActive_WhenEndDateIsNotMaxValue_ShouldReturnFalse()
    {
        // Arrange
        var goal = GoalFaker.Generate(endDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)));

        // Act
        var result = goal.IsActive();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void FullyOverlappedBy_WhenOtherGoalFullyOverlaps_ShouldReturnTrue()
    {
        // Arrange
        var goal = GoalFaker.Generate(
            startDate: DateOnly.FromDateTime(DateTime.UtcNow),
            endDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30))
        );
        var otherGoal = GoalFaker.Generate(
            startDate: goal.StartDate.AddDays(-1),
            endDate: goal.EndDate.AddDays(1)
        );

        // Act
        var result = goal.FullyOverlappedBy(otherGoal);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void FullyOverlappedBy_WhenOtherGoalDoesNotFullyOverlap_ShouldReturnFalse()
    {
        // Arrange
        var goal = GoalFaker.Generate(
            startDate: DateOnly.FromDateTime(DateTime.UtcNow),
            endDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30))
        );
        var otherGoal = GoalFaker.Generate(
            startDate: goal.StartDate.AddDays(1),
            endDate: goal.EndDate.AddDays(1)
        );

        // Act
        var result = goal.FullyOverlappedBy(otherGoal);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void HasSameDates_WhenDatesAreEqual_ShouldReturnTrue()
    {
        // Arrange
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var endDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30));
        var goal = GoalFaker.Generate(startDate: startDate, endDate: endDate);
        var otherGoal = GoalFaker.Generate(startDate: startDate, endDate: endDate);

        // Act
        var result = goal.HasSameDates(otherGoal);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void HasSameDates_WhenDatesAreDifferent_ShouldReturnFalse()
    {
        // Arrange
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var endDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30));
        var goal = GoalFaker.Generate(startDate: startDate, endDate: endDate);
        var otherGoal = GoalFaker.Generate(
            startDate: startDate.AddDays(1),
            endDate: endDate.AddDays(1)
        );

        // Act
        var result = goal.HasSameDates(otherGoal);

        // Assert
        result.Should().BeFalse();
    }
}
