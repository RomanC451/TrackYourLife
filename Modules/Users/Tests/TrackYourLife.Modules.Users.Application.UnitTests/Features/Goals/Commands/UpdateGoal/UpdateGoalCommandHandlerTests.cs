using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Services;
using TrackYourLife.Modules.Users.Application.Features.Goals.Commands.UpdateGoal;
using TrackYourLife.Modules.Users.Domain.Features.Goals;
using TrackYourLife.Modules.Users.Domain.UnitTests.Utils;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Enums;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Application.UnitTests.Features.Goals.Commands.UpdateGoal;

public class UpdateGoalCommandHandlerTests
{
    private readonly IGoalRepository _goalRepository;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IGoalsManagerService _goalsManagerService;
    private readonly ICommandHandler<UpdateGoalCommand> _handler;

    public UpdateGoalCommandHandlerTests()
    {
        _goalRepository = Substitute.For<IGoalRepository>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _goalsManagerService = Substitute.For<IGoalsManagerService>();
        _handler = new UpdateGoalCommandHandler(
            _goalRepository,
            _userIdentifierProvider,
            _goalsManagerService
        );
    }

    [Fact]
    public async Task Handle_WithValidInput_ReturnsSuccess()
    {
        // Arrange
        var userId = UserId.NewId();
        var goalId = GoalId.NewId();
        var command = new UpdateGoalCommand(
            goalId,
            GoalType.Calories,
            10000,
            GoalPeriod.Day,
            DateOnly.FromDateTime(DateTime.UtcNow),
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30))
        );
        var goal = GoalFaker.Generate(userId: userId);

        _userIdentifierProvider.UserId.Returns(userId);
        _goalRepository.GetByIdAsync(goalId, Arg.Any<CancellationToken>()).Returns(goal);
        _goalsManagerService
            .HandleOverlappingGoalsAsync(goal, command.Force, Arg.Any<CancellationToken>())
            .Returns(Result.Success(false));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _goalRepository.Received(1).Update(goal);
    }

    [Fact]
    public async Task Handle_WhenGoalNotFound_ReturnsFailure()
    {
        // Arrange
        var goalId = GoalId.NewId();
        var command = new UpdateGoalCommand(
            goalId,
            GoalType.Calories,
            10000,
            GoalPeriod.Day,
            DateOnly.FromDateTime(DateTime.UtcNow),
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30))
        );

        _goalRepository.GetByIdAsync(goalId, Arg.Any<CancellationToken>()).Returns((Goal?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(GoalErrors.NotFound(goalId));
        _goalRepository.DidNotReceive().Update(Arg.Any<Goal>());
    }

    [Fact]
    public async Task Handle_WhenGoalNotOwnedByUser_ReturnsFailure()
    {
        // Arrange
        var userId = UserId.NewId();
        var otherUserId = UserId.NewId();
        var goalId = GoalId.NewId();
        var command = new UpdateGoalCommand(
            goalId,
            GoalType.Calories,
            10000,
            GoalPeriod.Day,
            DateOnly.FromDateTime(DateTime.UtcNow),
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30))
        );
        var goal = GoalFaker.Generate(userId: otherUserId);

        _userIdentifierProvider.UserId.Returns(userId);
        _goalRepository.GetByIdAsync(goalId, Arg.Any<CancellationToken>()).Returns(goal);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(GoalErrors.NotOwned(goalId));
        _goalRepository.DidNotReceive().Update(Arg.Any<Goal>());
    }

    [Fact]
    public async Task Handle_WhenOverlappingGoalsAndForceFalse_ReturnsFailure()
    {
        // Arrange
        var userId = UserId.NewId();
        var goalId = GoalId.NewId();
        var command = new UpdateGoalCommand(
            goalId,
            GoalType.Calories,
            10000,
            GoalPeriod.Day,
            DateOnly.FromDateTime(DateTime.UtcNow),
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)),
            false
        );
        var goal = GoalFaker.Generate(userId: userId);

        _userIdentifierProvider.UserId.Returns(userId);
        _goalRepository.GetByIdAsync(goalId, Arg.Any<CancellationToken>()).Returns(goal);
        _goalsManagerService
            .HandleOverlappingGoalsAsync(goal, command.Force, Arg.Any<CancellationToken>())
            .Returns(Result.Failure<bool>(GoalErrors.Overlapping(goal.Type)));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("UserGoal.Overlapping");
        _goalRepository.DidNotReceive().Update(Arg.Any<Goal>());
    }
}
