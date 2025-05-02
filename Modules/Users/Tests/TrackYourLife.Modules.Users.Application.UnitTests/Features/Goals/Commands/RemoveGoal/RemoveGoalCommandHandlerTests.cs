using FluentAssertions;
using NSubstitute;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Services;
using TrackYourLife.Modules.Users.Application.Features.Goals.Commands.RemoveGoal;
using TrackYourLife.Modules.Users.Domain.Features.Goals;
using TrackYourLife.Modules.Users.Domain.UnitTests.Utils;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;
using Xunit;

namespace TrackYourLife.Modules.Users.Application.UnitTests.Features.Goals.Commands.RemoveGoal;

public class RemoveGoalCommandHandlerTests
{
    private readonly IGoalRepository _goalRepository;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly ICommandHandler<RemoveGoalCommand> _handler;

    public RemoveGoalCommandHandlerTests()
    {
        _goalRepository = Substitute.For<IGoalRepository>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _handler = new RemoveGoalCommandHandler(_goalRepository, _userIdentifierProvider);
    }

    [Fact]
    public async Task Handle_WithValidInput_ReturnsSuccess()
    {
        // Arrange
        var userId = UserId.NewId();
        var goalId = GoalId.NewId();
        var command = new RemoveGoalCommand(goalId);
        var goal = GoalFaker.Generate(userId: userId);

        _userIdentifierProvider.UserId.Returns(userId);
        _goalRepository.GetByIdAsync(goalId, Arg.Any<CancellationToken>()).Returns(goal);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _goalRepository.Received(1).Remove(goal);
    }

    [Fact]
    public async Task Handle_WhenGoalNotFound_ReturnsFailure()
    {
        // Arrange
        var goalId = GoalId.NewId();
        var command = new RemoveGoalCommand(goalId);

        _goalRepository.GetByIdAsync(goalId, Arg.Any<CancellationToken>()).Returns((Goal?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(GoalErrors.NotFound(goalId));
        _goalRepository.DidNotReceive().Remove(Arg.Any<Goal>());
    }

    [Fact]
    public async Task Handle_WhenGoalNotOwnedByUser_ReturnsFailure()
    {
        // Arrange
        var userId = UserId.NewId();
        var otherUserId = UserId.NewId();
        var goalId = GoalId.NewId();
        var command = new RemoveGoalCommand(goalId);
        var goal = GoalFaker.Generate(userId: otherUserId);

        _userIdentifierProvider.UserId.Returns(userId);
        _goalRepository.GetByIdAsync(goalId, Arg.Any<CancellationToken>()).Returns(goal);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(GoalErrors.NotOwned(goalId));
        _goalRepository.DidNotReceive().Remove(Arg.Any<Goal>());
    }
}
