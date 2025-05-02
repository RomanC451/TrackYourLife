using FluentAssertions;
using NSubstitute;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Services;
using TrackYourLife.Modules.Users.Application.Features.Goals.Commands.AddGoal;
using TrackYourLife.Modules.Users.Domain.Features.Goals;
using TrackYourLife.Modules.Users.Domain.UnitTests.Utils;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Enums;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;
using Xunit;

namespace TrackYourLife.Modules.Users.Application.UnitTests.Features.Goals.Commands.AddGoal;

public class AddGoalCommandHandlerTests
{
    private readonly IGoalRepository _goalRepository;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IGoalsManagerService _goalsManagerService;
    private readonly AddGoalCommandHandler _handler;

    public AddGoalCommandHandlerTests()
    {
        _goalRepository = Substitute.For<IGoalRepository>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _goalsManagerService = Substitute.For<IGoalsManagerService>();
        _handler = new AddGoalCommandHandler(
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
        var command = new AddGoalCommand(
            Value: 100,
            Type: GoalType.Calories,
            PerPeriod: GoalPeriod.Day,
            StartDate: DateOnly.FromDateTime(DateTime.UtcNow),
            EndDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)),
            Force: false
        );

        _userIdentifierProvider.UserId.Returns(userId);
        _goalsManagerService
            .HandleOverlappingGoalsAsync(
                Arg.Any<Goal>(),
                command.Force,
                Arg.Any<CancellationToken>()
            )
            .Returns(Result.Success(false));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _goalRepository.Received(1).AddAsync(Arg.Any<Goal>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithOverlappingGoalsAndForceFalse_ReturnsFailure()
    {
        // Arrange
        var userId = UserId.NewId();
        var command = new AddGoalCommand(
            Value: 100,
            Type: GoalType.Calories,
            PerPeriod: GoalPeriod.Day,
            StartDate: DateOnly.FromDateTime(DateTime.UtcNow),
            EndDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)),
            Force: false
        );

        _userIdentifierProvider.UserId.Returns(userId);
        _goalsManagerService
            .HandleOverlappingGoalsAsync(
                Arg.Any<Goal>(),
                command.Force,
                Arg.Any<CancellationToken>()
            )
            .Returns(Result.Failure<bool>(GoalErrors.Overlapping(command.Type)));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(GoalErrors.Overlapping(command.Type));
        await _goalRepository
            .DidNotReceive()
            .AddAsync(Arg.Any<Goal>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithOverlappingGoalsAndForceTrue_ReturnsSuccess()
    {
        // Arrange
        var userId = UserId.NewId();
        var command = new AddGoalCommand(
            Value: 100,
            Type: GoalType.Calories,
            PerPeriod: GoalPeriod.Day,
            StartDate: DateOnly.FromDateTime(DateTime.UtcNow),
            EndDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)),
            Force: true
        );

        _userIdentifierProvider.UserId.Returns(userId);
        _goalsManagerService
            .HandleOverlappingGoalsAsync(
                Arg.Any<Goal>(),
                command.Force,
                Arg.Any<CancellationToken>()
            )
            .Returns(Result.Success(false));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _goalRepository.Received(1).AddAsync(Arg.Any<Goal>(), Arg.Any<CancellationToken>());
    }
}
