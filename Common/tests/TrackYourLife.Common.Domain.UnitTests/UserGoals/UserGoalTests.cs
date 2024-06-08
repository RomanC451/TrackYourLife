using System.Reflection;
using TrackYourLife.Common.Domain.Errors;
using TrackYourLife.Common.Domain.Shared;
using TrackYourLife.Common.Domain.UserGoals;
using TrackYourLife.Common.Domain.Users.StrongTypes;

namespace TrackYourLife.Common.Domain.UnitTests.UserGoals;

public class UserGoalTests
{
    private static Result<UserGoal> CreateGoal(int value, string startDate, string? endDate) =>
        UserGoal.Create(
            id: UserGoalId.NewId(),
            userId: UserId.NewId(),
            type: UserGoalType.Calories,
            value: value,
            perPeriod: UserGoalPerPeriod.Day,
            startDate: DateOnly.Parse(startDate),
            endDate: !string.IsNullOrEmpty(endDate) ? DateOnly.Parse(endDate) : null
        );

    [Fact]
    public void Create_WithValidData_ShouldCreateGoal()
    {
        // Act
        var result = CreateGoal(2000, "02-20-2024", "02-21-2024");

        // Assert
        Assert.True(result.IsSuccess);

        var goal = result.Value;

        Assert.NotNull(goal);
        Assert.Equal(2000, goal.Value);
        Assert.Equal(UserGoalPerPeriod.Day, goal.PerPeriod);
        Assert.Equal(DateOnly.Parse("02-20-2024"), goal.StartDate);
        Assert.Equal(DateOnly.Parse("02-21-2024"), goal.EndDate);
    }

    [Fact]
    public void Create_WithNullEndDate_ShouldCreateGoal()
    {
        // Act
        var result = CreateGoal(2000, "02-20-2024", null);

        // Assert
        Assert.True(result.IsSuccess);

        var goal = result.Value;

        Assert.NotNull(goal);
        Assert.Equal(2000, goal.Value);
        Assert.Equal(UserGoalPerPeriod.Day, goal.PerPeriod);
        Assert.Equal(DateOnly.Parse("02-20-2024"), goal.StartDate);
        Assert.Equal(DateOnly.MaxValue, goal.EndDate);
    }

    [Fact]
    public void Create_WithEndDateBeforeStartDate_ReturnsError()
    {
        // Act
        var result = CreateGoal(2000, "02-20-2024", "02-19-2024");

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(nameof(DomainErrors.ArgumentError.Custom), result.Error.Code);
    }

    [Fact]
    public void Create_WithInvalidId_ReturnsError()
    {
        // Act
        var result = UserGoal.Create(
            id: UserGoalId.Empty,
            userId: UserId.NewId(),
            type: UserGoalType.Calories,
            value: 2000,
            perPeriod: UserGoalPerPeriod.Day,
            startDate: DateOnly.Parse("02-01-2024"),
            endDate: DateOnly.Parse("02-19-2024")
        );

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(nameof(DomainErrors.ArgumentError.Empty), result.Error.Code);
    }

    [Fact]
    public void Create_WithInvalidUserId_ReturnsError()
    {
        // Act
        var result = UserGoal.Create(
            id: UserGoalId.NewId(),
            userId: UserId.Empty,
            type: UserGoalType.Calories,
            value: 2000,
            perPeriod: UserGoalPerPeriod.Day,
            startDate: DateOnly.Parse("02-01-2024"),
            endDate: DateOnly.Parse("02-19-2024")
        );

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(nameof(DomainErrors.ArgumentError.Empty), result.Error.Code);
    }

    [Fact]
    public void Create_WithInvalidStartDate_ReturnsError()
    {
        // Act
        var result = UserGoal.Create(
            id: UserGoalId.NewId(),
            userId: UserId.NewId(),
            type: UserGoalType.Calories,
            value: 2000,
            perPeriod: UserGoalPerPeriod.Day,
            startDate: default,
            endDate: DateOnly.Parse("02-19-2024")
        );

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(nameof(DomainErrors.ArgumentError.Empty), result.Error.Code);
    }

    [Fact]
    public void IsActive_WithEndDateIsNull_ShouldReturnTrue()
    {
        // Arrange
        var goal = CreateGoal(2000, "02-20-2024", null).Value;

        // Act
        var isActive = goal.IsActive();

        // Assert
        Assert.True(isActive);
    }

    [Fact]
    public void IsActive_WithEndDateIsNotNull_ShouldReturnFalse()
    {
        // Arrange
        var goal = CreateGoal(2000, "02-20-2024", "02-21-2024").Value;

        // Act
        var isActive = goal.IsActive();

        // Assert
        Assert.False(isActive);
    }

    [Theory]
    [InlineData("02-01-2024", "02-15-2024", "02-16-2024", "02-20-2024")]
    [InlineData("02-01-2024", "02-15-2024", "02-15-2024", "02-20-2024")]
    [InlineData("02-01-2024", "02-15-2024", "02-14-2024", "02-20-2024")]
    [InlineData("02-01-2024", "02-15-2024", "02-15-2024", null)]
    [InlineData("02-01-2024", null, "02-14-2024", "02-20-2024")]
    [InlineData("02-01-2024", null, "02-14-2024", null)]
    [InlineData("02-15-2024", "02-29-2024", "02-01-2024", "02-20-2024")]
    [InlineData("02-15-2024", null, "02-01-2024", "02-20-2024")]
    public void FullyOverlappedBy_WithNotOverlappingGoal_ShouldReturnFalse(
        string firstGoalStartDay,
        string? firstGoalEndDay,
        string secondGoalStartDay,
        string? secondGoalEndDay
    )
    {
        // Arrange
        var firstGoal = CreateGoal(2000, firstGoalStartDay, firstGoalEndDay).Value;
        var secondGoal = CreateGoal(2000, secondGoalStartDay, secondGoalEndDay).Value;

        // Act
        var isOverlapping = firstGoal.FullyOverlappedBy(secondGoal);

        // Assert
        Assert.False(isOverlapping);
    }

    [Theory]
    [InlineData("02-15-2024", "02-29-2024", "02-01-2024", "02-29-2024")]
    [InlineData("02-15-2024", "02-29-2024", "02-01-2024", null)]
    [InlineData("02-15-2024", "02-29-2024", "02-15-2024", null)]
    [InlineData("02-15-2024", "02-29-2024", "02-15-2024", "02-29-2024")]
    [InlineData("02-15-2024", null, "02-01-2024", null)]
    public void FullyOverlappedBy_WithOverlappingGoal_ShouldReturnTrue(
        string firstGoalStartDay,
        string? firstGoalEndDay,
        string secondGoalStartDay,
        string? secondGoalEndDay
    )
    {
        // Arrange
        var firstGoal = CreateGoal(2000, firstGoalStartDay, firstGoalEndDay).Value;
        var secondGoal = CreateGoal(2000, secondGoalStartDay, secondGoalEndDay).Value;

        // Act
        var isOverlapping = firstGoal.FullyOverlappedBy(secondGoal);

        // Assert
        Assert.True(isOverlapping);
    }
}
