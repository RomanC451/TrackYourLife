using TrackYourLife.Common.Domain.UserGoals;
using TrackYourLife.Common.Domain.Users.StrongTypes;
using TrackYourLife.Persistence.Specifications;

namespace TrackYourLife.Common.Persistence.UnitTests.Specifications;

public class UserGoalOverlappingOtherGoalTests
{
    private readonly UserId _userId = UserId.NewId();

    private UserGoal CreateGoal(int value, string startDate, string? endDate) =>
        UserGoal.Create(
            id: UserGoalId.NewId(),
            userId: _userId,
            type: UserGoalType.Calories,
            value: value,
            perPeriod: UserGoalPerPeriod.Day,
            startDate: DateOnly.Parse(startDate),
            endDate: !string.IsNullOrEmpty(endDate) ? DateOnly.Parse(endDate) : null
        ).Value;

    [Theory]
    [InlineData("01-25-2024", "02-5-2024", "02-01-2024", "02-29-2024")]
    [InlineData("02-25-2024", "03-5-2024", "02-01-2024", "02-29-2024")]
    [InlineData("02-25-2024", "02-27-2024", "02-01-2024", "02-29-2024")]

    [InlineData("02-01-2024", "02-01-2024", "02-01-2024", "02-29-2024")]

    [InlineData("02-29-2024", "02-29-2024", "02-01-2024", "02-29-2024")]
    [InlineData("02-01-2024", "02-29-2024", "02-01-2024", "02-29-2024")]




    [InlineData("01-25-2024", "02-5-2024", "02-01-2024", null)]
    [InlineData("02-03-2024", "02-5-2024", "02-01-2024", null)]
    [InlineData("01-25-2024", null, "02-01-2024", "02-29-2024")]

    [InlineData("01-25-2024", null, "02-01-2024", null)]
    [InlineData("02-25-2024", null, "02-01-2024", null)]

    public void Expression_WhenNewGoalOverlapsOldGOal_ReturnsTrue(
        string newGoalStartDay,
        string? newGoalEndDay,
        string dbGoalStartDay,
        string? dbGoalEndDay
    )
    {
        // Arrange
        var newGoal = CreateGoal(2000, newGoalStartDay, newGoalEndDay);
        var specification = new UserGoalOverlappingOtherGoalSpecification(newGoal);

        // Act
        var expression = specification.ToExpression();

        // Assert
        var dbGoal = CreateGoal(3000, dbGoalStartDay, dbGoalEndDay);
        Assert.True(expression.Compile().Invoke(dbGoal));
    }

    [Theory]
    [InlineData("01-25-2024", "01-31-2024", "02-01-2024", "02-29-2024")]
    [InlineData("01-25-2024", "01-31-2024", "02-01-2024", null)]
    [InlineData("03-01-2024", "03-01-2024", "02-01-2024", "02-29-2024")]
    [InlineData("03-01-2024", null, "02-01-2024", "02-29-2024")]


    public void Expression_WhenNewGoalDoNotOverlapsOldGoal_ReturnsFalse(
        string newGoalStartDay,
        string? newGoalEndDay,
        string oldGoalStartDay,
        string? oldGoalEndDay
    )
    {
        // Arrange
        var newGoal = CreateGoal(2000, newGoalStartDay, newGoalEndDay);
        var specification = new UserGoalOverlappingOtherGoalSpecification(newGoal);

        // Act
        var expression = specification.ToExpression();

        // Assert
        var oldGoal = CreateGoal(3000, oldGoalStartDay, oldGoalEndDay);
        Assert.False(expression.Compile().Invoke(oldGoal));
    }
}
