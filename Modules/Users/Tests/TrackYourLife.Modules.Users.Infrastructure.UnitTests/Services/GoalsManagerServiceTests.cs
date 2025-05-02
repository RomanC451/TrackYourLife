using TrackYourLife.Modules.Users.Domain.Features.Goals;
using TrackYourLife.Modules.Users.Domain.UnitTests.Utils;
using TrackYourLife.Modules.Users.Infrastructure.Services;

namespace TrackYourLife.Modules.Users.Infrastructure.UnitTests.Services;

public class GoalsManagerServiceTests
{
    private readonly IGoalRepository _goalRepository;
    private readonly GoalsManagerService _goalsManagerService;

    public GoalsManagerServiceTests()
    {
        _goalRepository = Substitute.For<IGoalRepository>();
        _goalsManagerService = new GoalsManagerService(_goalRepository);
    }

    [Fact]
    public async Task HandleOverlappingGoalsAsync_NoOverlappingGoals_ShouldReturnSuccess()
    {
        // Arrange
        var newGoal = GoalFaker.Generate(
            startDate: new DateOnly(2024, 1, 1),
            endDate: new DateOnly(2024, 1, 31)
        );

        _goalRepository
            .GetOverlappingGoalsAsync(newGoal, Arg.Any<CancellationToken>())
            .Returns(new List<Goal>());

        // Act
        var result = await _goalsManagerService.HandleOverlappingGoalsAsync(
            newGoal,
            false,
            CancellationToken.None
        );

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeFalse();
        _goalRepository.DidNotReceive().Update(Arg.Any<Goal>());
        _goalRepository.DidNotReceive().Remove(Arg.Any<Goal>());
    }

    [Fact]
    public async Task HandleOverlappingGoalsAsync_OverlappingGoalsWithoutForce_ShouldReturnFailure()
    {
        // Arrange
        var newGoal = GoalFaker.Generate(
            startDate: new DateOnly(2024, 1, 1),
            endDate: new DateOnly(2024, 1, 31)
        );

        var existingGoal = GoalFaker.Generate(
            startDate: new DateOnly(2024, 1, 15),
            endDate: new DateOnly(2024, 2, 15)
        );

        _goalRepository
            .GetOverlappingGoalsAsync(newGoal, Arg.Any<CancellationToken>())
            .Returns(new List<Goal> { existingGoal });

        // Act
        var result = await _goalsManagerService.HandleOverlappingGoalsAsync(
            newGoal,
            false,
            CancellationToken.None
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(GoalErrors.Overlapping(newGoal.Type));
        _goalRepository.DidNotReceive().Update(Arg.Any<Goal>());
        _goalRepository.DidNotReceive().Remove(Arg.Any<Goal>());
    }

    [Fact]
    public async Task HandleOverlappingGoalsAsync_IdenticalGoal_ShouldReturnSuccess()
    {
        // Arrange
        var newGoal = GoalFaker.Generate(
            startDate: new DateOnly(2024, 1, 1),
            endDate: new DateOnly(2024, 1, 31),
            value: 100
        );

        var existingGoal = GoalFaker.Generate(
            startDate: new DateOnly(2024, 1, 1),
            endDate: new DateOnly(2024, 1, 31),
            value: 100
        );

        _goalRepository
            .GetOverlappingGoalsAsync(newGoal, Arg.Any<CancellationToken>())
            .Returns(new List<Goal> { existingGoal });

        // Act
        var result = await _goalsManagerService.HandleOverlappingGoalsAsync(
            newGoal,
            true,
            CancellationToken.None
        );

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
        _goalRepository.DidNotReceive().Update(Arg.Any<Goal>());
        _goalRepository.DidNotReceive().Remove(Arg.Any<Goal>());
    }

    [Fact]
    public async Task HandleOverlappingGoalsAsync_FullyOverlappingGoal_ShouldRemoveExistingGoal()
    {
        // Arrange
        var newGoal = GoalFaker.Generate(
            startDate: new DateOnly(2024, 1, 1),
            endDate: new DateOnly(2024, 3, 31)
        );

        var existingGoal = GoalFaker.Generate(
            startDate: new DateOnly(2024, 1, 15),
            endDate: new DateOnly(2024, 2, 15)
        );

        _goalRepository
            .GetOverlappingGoalsAsync(newGoal, Arg.Any<CancellationToken>())
            .Returns(new List<Goal> { existingGoal });

        // Act
        var result = await _goalsManagerService.HandleOverlappingGoalsAsync(
            newGoal,
            true,
            CancellationToken.None
        );

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeFalse();
        _goalRepository.Received(1).Remove(existingGoal);
        _goalRepository.DidNotReceive().Update(Arg.Any<Goal>());
    }

    [Fact]
    public async Task HandleOverlappingGoalsAsync_OverlappingAtStart_ShouldUpdateExistingGoal()
    {
        // Arrange
        var newGoal = GoalFaker.Generate(
            startDate: new DateOnly(2024, 2, 1),
            endDate: new DateOnly(2024, 3, 31)
        );

        var existingGoal = GoalFaker.Generate(
            startDate: new DateOnly(2024, 1, 1),
            endDate: new DateOnly(2024, 2, 15)
        );

        _goalRepository
            .GetOverlappingGoalsAsync(newGoal, Arg.Any<CancellationToken>())
            .Returns(new List<Goal> { existingGoal });

        // Act
        var result = await _goalsManagerService.HandleOverlappingGoalsAsync(
            newGoal,
            true,
            CancellationToken.None
        );

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeFalse();
        _goalRepository.Received(1).Update(existingGoal);
        _goalRepository.DidNotReceive().Remove(Arg.Any<Goal>());
        existingGoal.EndDate.Should().Be(newGoal.StartDate.AddDays(-1));
    }

    [Fact]
    public async Task HandleOverlappingGoalsAsync_OverlappingAtEnd_ShouldUpdateExistingGoal()
    {
        // Arrange
        var newGoal = GoalFaker.Generate(
            startDate: new DateOnly(2024, 1, 1),
            endDate: new DateOnly(2024, 2, 15)
        );

        var existingGoal = GoalFaker.Generate(
            startDate: new DateOnly(2024, 2, 1),
            endDate: new DateOnly(2024, 3, 31)
        );

        _goalRepository
            .GetOverlappingGoalsAsync(newGoal, Arg.Any<CancellationToken>())
            .Returns(new List<Goal> { existingGoal });

        // Act
        var result = await _goalsManagerService.HandleOverlappingGoalsAsync(
            newGoal,
            true,
            CancellationToken.None
        );

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeFalse();
        _goalRepository.Received(1).Update(existingGoal);
        _goalRepository.DidNotReceive().Remove(Arg.Any<Goal>());
        existingGoal.StartDate.Should().Be(newGoal.EndDate.AddDays(1));
    }
}
