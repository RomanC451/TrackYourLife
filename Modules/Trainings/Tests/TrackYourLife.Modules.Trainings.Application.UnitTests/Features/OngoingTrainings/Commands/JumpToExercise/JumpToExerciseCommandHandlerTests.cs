using TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Commands.JumpToExercise;
using TrackYourLife.Modules.Trainings.Application.UnitTests.Utils;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.OngoingTrainings.Commands.JumpToExercise;

public class JumpToExerciseCommandHandlerTests
{
    private readonly IOngoingTrainingsRepository _ongoingTrainingsRepository;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly JumpToExerciseCommandHandler _handler;

    private readonly UserId _userId;
    private readonly OngoingTrainingId _ongoingTrainingId;

    public JumpToExerciseCommandHandlerTests()
    {
        _ongoingTrainingsRepository = Substitute.For<IOngoingTrainingsRepository>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _handler = new JumpToExerciseCommandHandler(
            _ongoingTrainingsRepository,
            _userIdentifierProvider
        );

        _userId = UserId.NewId();
        _ongoingTrainingId = OngoingTrainingId.NewId();

        _userIdentifierProvider.UserId.Returns(_userId);
    }

    [Fact]
    public async Task Handle_WhenOngoingTrainingNotFound_ShouldReturnFailure()
    {
        // Arrange
        _ongoingTrainingsRepository
            .GetByIdAsync(_ongoingTrainingId, Arg.Any<CancellationToken>())
            .Returns((OngoingTraining?)null);

        var command = new JumpToExerciseCommand(_ongoingTrainingId, 1);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OngoingTrainingErrors.NotFoundById(_ongoingTrainingId));
        _ongoingTrainingsRepository.DidNotReceive().Update(Arg.Any<OngoingTraining>());
    }

    [Fact]
    public async Task Handle_WhenOngoingTrainingBelongsToDifferentUser_ShouldReturnFailure()
    {
        // Arrange
        var differentUserId = UserId.NewId();
        var ongoingTraining = OngoingTrainingFaker.Generate(
            id: _ongoingTrainingId,
            userId: differentUserId
        );

        _ongoingTrainingsRepository
            .GetByIdAsync(_ongoingTrainingId, Arg.Any<CancellationToken>())
            .Returns(ongoingTraining);

        var command = new JumpToExerciseCommand(_ongoingTrainingId, 1);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OngoingTrainingErrors.NotOwned(_ongoingTrainingId));
        _ongoingTrainingsRepository.DidNotReceive().Update(Arg.Any<OngoingTraining>());
    }

    [Fact]
    public async Task Handle_WhenOngoingTrainingIsFinished_ShouldReturnFailure()
    {
        // Arrange
        var ongoingTraining = OngoingTrainingFaker.Generate(
            id: _ongoingTrainingId,
            userId: _userId
        );
        ongoingTraining.Finish(DateTime.UtcNow, null);

        _ongoingTrainingsRepository
            .GetByIdAsync(_ongoingTrainingId, Arg.Any<CancellationToken>())
            .Returns(ongoingTraining);

        var command = new JumpToExerciseCommand(_ongoingTrainingId, 1);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OngoingTrainingErrors.AlreadyFinished(_ongoingTrainingId));
        _ongoingTrainingsRepository.DidNotReceive().Update(Arg.Any<OngoingTraining>());
    }

    [Fact]
    public async Task Handle_WhenExerciseIndexIsInvalid_ShouldReturnFailure()
    {
        // Arrange
        var ongoingTraining = OngoingTrainingFaker.Generate(
            id: _ongoingTrainingId,
            userId: _userId
        );

        _ongoingTrainingsRepository
            .GetByIdAsync(_ongoingTrainingId, Arg.Any<CancellationToken>())
            .Returns(ongoingTraining);

        // Use an invalid exercise index (out of bounds)
        var invalidExerciseIndex = 999;
        var command = new JumpToExerciseCommand(_ongoingTrainingId, invalidExerciseIndex);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
        _ongoingTrainingsRepository.DidNotReceive().Update(Arg.Any<OngoingTraining>());
    }

    [Fact]
    public async Task Handle_WhenAllValidationsPass_ShouldUpdateOngoingTraining()
    {
        // Arrange
        var ongoingTraining = OngoingTrainingFaker.Generate(
            id: _ongoingTrainingId,
            userId: _userId
        );

        _ongoingTrainingsRepository
            .GetByIdAsync(_ongoingTrainingId, Arg.Any<CancellationToken>())
            .Returns(ongoingTraining);

        var targetExerciseIndex = 0; // Jump to first exercise
        var command = new JumpToExerciseCommand(_ongoingTrainingId, targetExerciseIndex);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        ongoingTraining.ExerciseIndex.Should().Be(targetExerciseIndex);
        ongoingTraining.SetIndex.Should().Be(0); // Should reset to first set
        _ongoingTrainingsRepository.Received(1).Update(ongoingTraining);
    }
}
