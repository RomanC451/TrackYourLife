using TrackYourLife.Modules.Users.Domain.UnitTests.Utils;
using TrackYourLife.Modules.Users.Infrastructure.Data.Goals;
using TrackYourLife.SharedLib.Domain.Enums;

namespace TrackYourLife.Modules.Users.Infrastructure.UnitTests.Data.Goals;

public class GoalQueryTests(DatabaseFixture fixture) : BaseRepositoryTests(fixture)
{
    private GoalQuery _sut = null!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _sut = new GoalQuery(ReadDbContext);
    }

    [Fact]
    public async Task GetGoalByTypeAndDateAsync_WhenGoalExists_ReturnsGoal()
    {
        // Arrange
        var user = UserFaker.Generate();
        await WriteDbContext.Users.AddAsync(user);
        var goal = GoalFaker.Generate(
            userId: user.Id,
            type: GoalType.Calories,
            startDate: new DateOnly(2024, 1, 1),
            endDate: new DateOnly(2024, 1, 31)
        );

        await WriteDbContext.Goals.AddAsync(goal);
        await WriteDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _sut.GetGoalByTypeAndDateAsync(
                user.Id,
                GoalType.Calories,
                new DateOnly(2024, 1, 15),
                CancellationToken.None
            );

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(goal.Id);
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetGoalByTypeAndDateAsync_WhenGoalDoesNotExist_ReturnsNull()
    {
        // Arrange
        var user = UserFaker.Generate();
        await WriteDbContext.Users.AddAsync(user);
        await WriteDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _sut.GetGoalByTypeAndDateAsync(
                user.Id,
                GoalType.Calories,
                new DateOnly(2024, 1, 15),
                CancellationToken.None
            );

            // Assert
            result.Should().BeNull();
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetGoalByTypeAndDateAsync_WhenDateIsOutsideGoalRange_ReturnsNull()
    {
        // Arrange
        var user = UserFaker.Generate();
        await WriteDbContext.Users.AddAsync(user);
        var goal = GoalFaker.Generate(
            userId: user.Id,
            startDate: new DateOnly(2024, 1, 1),
            endDate: new DateOnly(2024, 1, 31)
        );
        await WriteDbContext.Goals.AddAsync(goal);

        await WriteDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _sut.GetGoalByTypeAndDateAsync(
                user.Id,
                GoalType.Calories,
                new DateOnly(2024, 2, 1),
                CancellationToken.None
            );

            // Assert
            result.Should().BeNull();
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }
}
