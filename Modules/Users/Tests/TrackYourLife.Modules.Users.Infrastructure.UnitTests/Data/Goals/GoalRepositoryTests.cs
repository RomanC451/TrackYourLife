using FluentAssertions;
using TrackYourLife.Modules.Users.Domain.Features.Goals;
using TrackYourLife.Modules.Users.Domain.UnitTests.Utils;
using TrackYourLife.Modules.Users.Infrastructure.Data.Goals;
using TrackYourLife.SharedLib.Domain.Enums;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Users.Infrastructure.UnitTests.Data.Goals;

[Collection("UsersRepositoryTests")]
public class GoalRepositoryTests : BaseRepositoryTests
{
    private GoalRepository _repository = null!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _repository = new GoalRepository(_writeDbContext!);
    }

    [Fact]
    public async Task GetOverlappingGoalsAsync_WhenNoOverlappingGoals_ReturnsEmptyList()
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
        try
        {
            // Act
            var result = await _repository.GetOverlappingGoalsAsync(goal, CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetOverlappingGoalsAsync_WhenOverlappingGoalsExist_ReturnsOverlappingGoals()
    {
        // Arrange
        var user = UserFaker.Generate();
        await _writeDbContext.Users.AddAsync(user);
        var existingGoal = GoalFaker.Generate(
            userId: user.Id,
            type: GoalType.Calories,
            startDate: new DateOnly(2024, 1, 15),
            endDate: new DateOnly(2024, 2, 15)
        );
        await _writeDbContext.UserGoals.AddAsync(existingGoal);
        await _writeDbContext.SaveChangesAsync();

        var newGoal = GoalFaker.Generate(
            userId: user.Id,
            type: GoalType.Calories,
            startDate: new DateOnly(2024, 2, 14),
            endDate: new DateOnly(2024, 3, 15)
        );

        try
        {
            // Act
            var result = await _repository.GetOverlappingGoalsAsync(
                newGoal,
                CancellationToken.None
            );

            // Assert
            result.Should().HaveCount(1);
            result[0].Id.Should().Be(existingGoal.Id);
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetGoalByUserIdAndTypeAsync_WhenGoalExists_ReturnsGoal()
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
        await _writeDbContext.UserGoals.AddAsync(goal);
        await _writeDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _repository.GetGoalByUserIdAndTypeAsync(
                user.Id,
                GoalType.Calories,
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
    public async Task GetGoalByUserIdAndTypeAsync_WhenGoalDoesNotExist_ReturnsNull()
    {
        // Arrange
        var user = UserFaker.Generate();
        await _writeDbContext.Users.AddAsync(user);
        await _writeDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _repository.GetGoalByUserIdAndTypeAsync(
                user.Id,
                GoalType.Calories,
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
