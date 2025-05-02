using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Services;
using TrackYourLife.Modules.Users.Application.Features.Goals.Commands.UpdateNutritionGoals;
using TrackYourLife.Modules.Users.Domain.Features.Goals;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Enums;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Application.UnitTests.Features.Goals.Commands.UpdateNutritionGoals;

public class UpdateNutritionGoalsCommandHandlerTests
{
    private readonly IGoalRepository _goalRepository;
    private readonly IGoalsManagerService _goalsManagerService;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly ICommandHandler<UpdateNutritionGoalsCommand> _handler;

    public UpdateNutritionGoalsCommandHandlerTests()
    {
        _goalRepository = Substitute.For<IGoalRepository>();
        _goalsManagerService = Substitute.For<IGoalsManagerService>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _handler = new UpdateNutritionGoalsCommandHandler(
            _goalRepository,
            _goalsManagerService,
            _userIdentifierProvider
        );
    }

    [Fact]
    public async Task Handle_WithValidInput_ReturnsSuccess()
    {
        // Arrange
        var userId = UserId.NewId();
        var command = new UpdateNutritionGoalsCommand(
            Calories: 2000,
            Protein: 150,
            Carbohydrates: 250,
            Fats: 70,
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
        await _goalRepository.Received(4).AddAsync(Arg.Any<Goal>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenOverlappingGoalsAndForceFalse_ReturnsFailure()
    {
        // Arrange
        var userId = UserId.NewId();
        var command = new UpdateNutritionGoalsCommand(
            Calories: 2000,
            Protein: 150,
            Carbohydrates: 250,
            Fats: 70,
            Force: false
        );

        _userIdentifierProvider.UserId.Returns(userId);
        _goalsManagerService
            .HandleOverlappingGoalsAsync(
                Arg.Any<Goal>(),
                command.Force,
                Arg.Any<CancellationToken>()
            )
            .Returns(Result.Failure<bool>(GoalErrors.Overlapping(GoalType.Calories)));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(GoalErrors.Overlapping(GoalType.Calories));
        await _goalRepository
            .DidNotReceive()
            .AddAsync(Arg.Any<Goal>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenOverlappingGoalsAndForceTrue_ReturnsSuccess()
    {
        // Arrange
        var userId = UserId.NewId();
        var command = new UpdateNutritionGoalsCommand(
            Calories: 2000,
            Protein: 150,
            Carbohydrates: 250,
            Fats: 70,
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
        await _goalRepository.Received(4).AddAsync(Arg.Any<Goal>(), Arg.Any<CancellationToken>());
    }
}
