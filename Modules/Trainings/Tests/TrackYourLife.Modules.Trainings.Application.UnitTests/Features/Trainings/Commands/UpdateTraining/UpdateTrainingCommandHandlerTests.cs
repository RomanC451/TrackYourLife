using TrackYourLife.Modules.Trainings.Application.Features.Trainings.Commands.UpdateTraining;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.Trainings.Commands.UpdateTraining;

public class UpdateTrainingCommandHandlerTests
{
    private readonly ITrainingsRepository _trainingsRepository;
    private readonly IOngoingTrainingsQuery _ongoingTrainingsQuery;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IExercisesRepository _exerciseRepository;
    private readonly UpdateTrainingCommandHandler _handler;

    private readonly UserId _userId;
    private readonly TrainingId _trainingId;
    private readonly string _name;
    private readonly List<string> _muscleGroups;
    private readonly Difficulty _difficulty;
    private readonly List<ExerciseId> _exerciseIds;
    private readonly int _duration;
    private readonly int _restSeconds;
    private readonly string? _description;

    public UpdateTrainingCommandHandlerTests()
    {
        _trainingsRepository = Substitute.For<ITrainingsRepository>();
        _ongoingTrainingsQuery = Substitute.For<IOngoingTrainingsQuery>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _exerciseRepository = Substitute.For<IExercisesRepository>();
        _handler = new UpdateTrainingCommandHandler(
            _trainingsRepository,
            _ongoingTrainingsQuery,
            _userIdentifierProvider,
            _exerciseRepository
        );

        _userId = UserId.NewId();
        _trainingId = TrainingId.NewId();
        _name = "Updated Training";
        _muscleGroups = ["Chest", "Triceps"];
        _difficulty = Difficulty.Medium;
        _exerciseIds = [ExerciseId.NewId(), ExerciseId.NewId()];
        _duration = 90;
        _restSeconds = 120;
        _description = "Updated description";

        _userIdentifierProvider.UserId.Returns(_userId);
    }

    [Fact]
    public async Task Handle_WhenTrainingNotFound_ShouldReturnFailure()
    {
        // Arrange
        _trainingsRepository
            .GetByIdAsync(_trainingId, Arg.Any<CancellationToken>())
            .Returns((Training?)null);

        var command = new UpdateTrainingCommand(
            _trainingId,
            _name,
            _muscleGroups,
            _difficulty,
            _duration,
            _restSeconds,
            _description,
            _exerciseIds
        );

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TrainingsErrors.NotFoundById(_trainingId));
        _trainingsRepository.DidNotReceive().Update(Arg.Any<Training>());
    }

    [Fact]
    public async Task Handle_WhenTrainingBelongsToDifferentUser_ShouldReturnFailure()
    {
        // Arrange
        var differentUserId = UserId.NewId();
        var training = TrainingFaker.Generate(id: _trainingId, userId: differentUserId);

        _trainingsRepository
            .GetByIdAsync(_trainingId, Arg.Any<CancellationToken>())
            .Returns(training);

        var command = new UpdateTrainingCommand(
            _trainingId,
            _name,
            _muscleGroups,
            _difficulty,
            _duration,
            _restSeconds,
            _description,
            _exerciseIds
        );

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TrainingsErrors.NotOwned(_trainingId));
        _trainingsRepository.DidNotReceive().Update(Arg.Any<Training>());
    }

    [Fact]
    public async Task Handle_WhenTrainingIsOngoing_ShouldReturnFailure()
    {
        // Arrange
        var training = TrainingFaker.Generate(id: _trainingId, userId: _userId);

        _trainingsRepository
            .GetByIdAsync(_trainingId, Arg.Any<CancellationToken>())
            .Returns(training);
        _ongoingTrainingsQuery
            .IsTrainingOngoingAsync(_trainingId, Arg.Any<CancellationToken>())
            .Returns(true);

        var command = new UpdateTrainingCommand(
            _trainingId,
            _name,
            _muscleGroups,
            _difficulty,
            _duration,
            _restSeconds,
            _description,
            _exerciseIds
        );

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TrainingsErrors.OngoingTraining(_trainingId));
        _trainingsRepository.DidNotReceive().Update(Arg.Any<Training>());
    }

    [Fact]
    public async Task Handle_WhenTrainingExerciseCreationFails_ShouldReturnFailure()
    {
        // Arrange
        var training = TrainingFaker.Generate(id: _trainingId, userId: _userId);
        // Return fewer exercises than requested to simulate missing exercises
        var exercises = new List<Exercise> { ExerciseFaker.Generate(id: _exerciseIds[0]) };

        _trainingsRepository
            .GetByIdAsync(_trainingId, Arg.Any<CancellationToken>())
            .Returns(training);
        _ongoingTrainingsQuery
            .IsTrainingOngoingAsync(_trainingId, Arg.Any<CancellationToken>())
            .Returns(false);
        _exerciseRepository
            .GetEnumerableWithinIdsCollectionAsync(_exerciseIds, Arg.Any<CancellationToken>())
            .Returns(exercises);

        var command = new UpdateTrainingCommand(
            _trainingId,
            _name,
            _muscleGroups,
            _difficulty,
            _duration,
            _restSeconds,
            _description,
            _exerciseIds
        );

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        _trainingsRepository.DidNotReceive().Update(Arg.Any<Training>());
    }

    [Fact]
    public async Task Handle_WhenAllValidationsPass_ShouldUpdateTraining()
    {
        // Arrange
        var training = TrainingFaker.Generate(id: _trainingId, userId: _userId);
        var exercises = _exerciseIds.Select(id => ExerciseFaker.Generate(id: id)).ToList();

        _trainingsRepository
            .GetByIdAsync(_trainingId, Arg.Any<CancellationToken>())
            .Returns(training);
        _ongoingTrainingsQuery
            .IsTrainingOngoingAsync(_trainingId, Arg.Any<CancellationToken>())
            .Returns(false);
        _exerciseRepository
            .GetEnumerableWithinIdsCollectionAsync(_exerciseIds, Arg.Any<CancellationToken>())
            .Returns(exercises);

        var command = new UpdateTrainingCommand(
            _trainingId,
            _name,
            _muscleGroups,
            _difficulty,
            _duration,
            _restSeconds,
            _description,
            _exerciseIds
        );

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _trainingsRepository.Received(1).Update(training);
    }
}
