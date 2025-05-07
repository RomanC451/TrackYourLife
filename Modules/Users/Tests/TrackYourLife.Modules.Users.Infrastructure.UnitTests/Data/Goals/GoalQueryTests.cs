using TrackYourLife.Modules.Users.Domain.Features.Goals;
using TrackYourLife.Modules.Users.Domain.UnitTests.Utils;
using TrackYourLife.Modules.Users.Infrastructure.Data.Goals;
using TrackYourLife.SharedLib.Domain.Enums;

namespace TrackYourLife.Modules.Users.Infrastructure.UnitTests.Data.Goals;

[Collection("UsersRepositoryTests")]
public class GoalQueryTests : BaseRepositoryTests
{
    private GoalQuery _sut = null!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _sut = new GoalQuery(_readDbContext!);
    }

    [Fact]
    public async Task GetGoalByTypeAndDateAsync_WhenGoalExists_ReturnsGoal()
    {
        // Arrange
        var user = UserFaker.Generate();
        await _writeDbContext.Users.AddAsync(user);
        var goal = GoalFaker.Generate(
            userId: user.Id,
            type: GoalType.Calories,
            startDate: new DateOnly(2024, 1, 1),
            endDate: new DateOnly(2024, 1, 31)
        );

        await _writeDbContext.Goals.AddAsync(goal);
        await _writeDbContext.SaveChangesAsync();

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
        await _writeDbContext.Users.AddAsync(user);
        await _writeDbContext.SaveChangesAsync();

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
        await _writeDbContext.Users.AddAsync(user);
        var goal = GoalFaker.Generate(
            userId: user.Id,
            startDate: new DateOnly(2024, 1, 1),
            endDate: new DateOnly(2024, 1, 31)
        );
        await _writeDbContext.Goals.AddAsync(goal);

        await _writeDbContext.SaveChangesAsync();

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
