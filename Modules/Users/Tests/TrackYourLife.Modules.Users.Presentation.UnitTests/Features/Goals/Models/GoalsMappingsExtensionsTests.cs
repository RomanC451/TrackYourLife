using TrackYourLife.Modules.Users.Domain.Features.Goals;
using TrackYourLife.Modules.Users.Presentation.Features.Goals;
using TrackYourLife.SharedLib.Domain.Enums;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Users.Presentation.UnitTests.Features.Goals.Models;

public class GoalsMappingsExtensionsTests
{
    [Fact]
    public void ToDto_WithGoalReadModel_ShouldMapCorrectly()
    {
        // Arrange
        var goalId = GoalId.NewId();
        var userId = UserId.NewId();
        var startDate = new DateOnly(2024, 1, 1);
        var endDate = new DateOnly(2024, 1, 31);

        var goalReadModel = new GoalReadModel(
            Id: goalId,
            UserId: userId,
            Value: 2000,
            Type: GoalType.Calories,
            Period: GoalPeriod.Day,
            StartDate: startDate,
            EndDate: endDate
        );

        // Act
        var dto = goalReadModel.ToDto();

        // Assert
        dto.Should().NotBeNull();
        dto.Id.Should().Be(goalId);
        dto.Type.Should().Be(GoalType.Calories);
        dto.Value.Should().Be(2000);
        dto.Period.Should().Be(GoalPeriod.Day);
        dto.StartDate.Should().Be(startDate);
        dto.EndDate.Should().Be(endDate);
    }

    [Fact]
    public void ToDto_WithDifferentGoalType_ShouldMapCorrectly()
    {
        // Arrange
        var goalId = GoalId.NewId();
        var userId = UserId.NewId();
        var startDate = new DateOnly(2024, 1, 1);
        var endDate = new DateOnly(2024, 1, 31);

        var goalReadModel = new GoalReadModel(
            Id: goalId,
            UserId: userId,
            Value: 150,
            Type: GoalType.Protein,
            Period: GoalPeriod.Week,
            StartDate: startDate,
            EndDate: endDate
        );

        // Act
        var dto = goalReadModel.ToDto();

        // Assert
        dto.Should().NotBeNull();
        dto.Id.Should().Be(goalId);
        dto.Type.Should().Be(GoalType.Protein);
        dto.Value.Should().Be(150);
        dto.Period.Should().Be(GoalPeriod.Week);
        dto.StartDate.Should().Be(startDate);
        dto.EndDate.Should().Be(endDate);
    }
}
