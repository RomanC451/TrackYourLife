using TrackYourLife.Modules.Trainings.Application.Features.Exercises.Commands.DeleteExercise;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.Exercises.Commands.DeleteExercise;

public class DeleteExerciseCommandHandlerTests
{
    private readonly IExercisesRepository _exerciseRepository;
    private readonly ITrainingsRepository _trainingsRepository;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly DeleteExerciseCommandHandler _handler;

    private readonly UserId _userId;
    private readonly ExerciseId _exerciseId;

    public DeleteExerciseCommandHandlerTests()
    {
        _exerciseRepository = Substitute.For<IExercisesRepository>();
        _trainingsRepository = Substitute.For<ITrainingsRepository>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _handler = new DeleteExerciseCommandHandler(
            _exerciseRepository,
            _trainingsRepository,
            _userIdentifierProvider
        );

        _userId = UserId.NewId();
        _exerciseId = ExerciseId.NewId();

        _userIdentifierProvider.UserId.Returns(_userId);
    }

    [Fact]
    public async Task Handle_WhenExerciseNotFound_ShouldReturnFailure()
    {
        // Arrange
        _exerciseRepository
            .GetByIdAsync(_exerciseId, Arg.Any<CancellationToken>())
            .Returns((Exercise?)null);

        var command = new DeleteExerciseCommand(_exerciseId, false);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ExercisesErrors.NotFoundById(_exerciseId));
        _exerciseRepository.DidNotReceive().Remove(Arg.Any<Exercise>());
    }

    [Fact]
    public async Task Handle_WhenExerciseBelongsToDifferentUser_ShouldReturnFailure()
    {
        // Arrange
        var differentUserId = UserId.NewId();
        var exercise = ExerciseFaker.Generate(id: _exerciseId, userId: differentUserId);

        _exerciseRepository
            .GetByIdAsync(_exerciseId, Arg.Any<CancellationToken>())
            .Returns(exercise);

        var command = new DeleteExerciseCommand(_exerciseId, false);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ExercisesErrors.NotFoundById(_exerciseId));
        _exerciseRepository.DidNotReceive().Remove(Arg.Any<Exercise>());
    }

    [Fact]
    public async Task Handle_WhenExerciseUsedInTrainingsAndForceDeleteFalse_ShouldReturnFailure()
    {
        // Arrange
        var exercise = ExerciseFaker.Generate(id: _exerciseId, userId: _userId);
        var trainings = new List<Training> { TrainingFaker.Generate() };

        _exerciseRepository
            .GetByIdAsync(_exerciseId, Arg.Any<CancellationToken>())
            .Returns(exercise);
        _trainingsRepository
            .GetThatContainsExerciseAsync(_exerciseId, Arg.Any<CancellationToken>())
            .Returns(trainings);

        var command = new DeleteExerciseCommand(_exerciseId, false);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ExercisesErrors.UsedInTrainings(_exerciseId));
        _exerciseRepository.DidNotReceive().Remove(Arg.Any<Exercise>());
    }

    [Fact]
    public async Task Handle_WhenExerciseUsedInTrainingsAndForceDeleteTrue_ShouldRemoveExerciseFromTrainings()
    {
        // Arrange
        var exercise = ExerciseFaker.Generate(id: _exerciseId, userId: _userId);
        var training = TrainingFaker.Generate();
        var trainings = new List<Training> { training };

        _exerciseRepository
            .GetByIdAsync(_exerciseId, Arg.Any<CancellationToken>())
            .Returns(exercise);
        _trainingsRepository
            .GetThatContainsExerciseAsync(_exerciseId, Arg.Any<CancellationToken>())
            .Returns(trainings);

        var command = new DeleteExerciseCommand(_exerciseId, true);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _trainingsRepository.Received(1).UpdateRange(Arg.Is<List<Training>>(t => t.Count == 1));
        _exerciseRepository.Received(1).Remove(exercise);
    }

    [Fact]
    public async Task Handle_WhenExerciseNotUsedInTrainings_ShouldDeleteExercise()
    {
        // Arrange
        var exercise = ExerciseFaker.Generate(id: _exerciseId, userId: _userId);

        _exerciseRepository
            .GetByIdAsync(_exerciseId, Arg.Any<CancellationToken>())
            .Returns(exercise);
        _trainingsRepository
            .GetThatContainsExerciseAsync(_exerciseId, Arg.Any<CancellationToken>())
            .Returns([]);

        var command = new DeleteExerciseCommand(_exerciseId, false);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _trainingsRepository.DidNotReceive().UpdateRange(Arg.Any<List<Training>>());
        _exerciseRepository.Received(1).Remove(exercise);
    }
}

