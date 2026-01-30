using TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Commands.SkipExercise;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.OngoingTrainings.Commands.SkipExercise;

public class SkipExerciseCommandHandlerTests
{
    private readonly IOngoingTrainingsRepository _ongoingTrainingsRepository;
    private readonly IExercisesHistoriesRepository _exercisesHistoriesRepository;
    private readonly IExercisesHistoriesQuery _exercisesHistoriesQuery;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly SkipExerciseCommandHandler _handler;

    private readonly UserId _userId;
    private readonly OngoingTrainingId _ongoingTrainingId;

    public SkipExerciseCommandHandlerTests()
    {
        _ongoingTrainingsRepository = Substitute.For<IOngoingTrainingsRepository>();
        _exercisesHistoriesRepository = Substitute.For<IExercisesHistoriesRepository>();
        _exercisesHistoriesQuery = Substitute.For<IExercisesHistoriesQuery>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _dateTimeProvider = Substitute.For<IDateTimeProvider>();
        _handler = new SkipExerciseCommandHandler(
            _ongoingTrainingsRepository,
            _exercisesHistoriesRepository,
            _exercisesHistoriesQuery,
            _userIdentifierProvider,
            _dateTimeProvider
        );

        _userId = UserId.NewId();
        _ongoingTrainingId = OngoingTrainingId.NewId();

        _userIdentifierProvider.UserId.Returns(_userId);
        _dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);
    }

    [Fact]
    public async Task Handle_WhenOngoingTrainingNotFound_ShouldReturnFailure()
    {
        // Arrange
        _ongoingTrainingsRepository
            .GetByIdAsync(_ongoingTrainingId, Arg.Any<CancellationToken>())
            .Returns((OngoingTraining?)null);

        var command = new SkipExerciseCommand(_ongoingTrainingId);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OngoingTrainingErrors.NotFoundById(_ongoingTrainingId));
        await _exercisesHistoriesRepository
            .DidNotReceive()
            .AddAsync(Arg.Any<ExerciseHistory>(), Arg.Any<CancellationToken>());
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

        var command = new SkipExerciseCommand(_ongoingTrainingId);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OngoingTrainingErrors.NotOwned(_ongoingTrainingId));
        await _exercisesHistoriesRepository
            .DidNotReceive()
            .AddAsync(Arg.Any<ExerciseHistory>(), Arg.Any<CancellationToken>());
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

        var command = new SkipExerciseCommand(_ongoingTrainingId);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OngoingTrainingErrors.AlreadyFinished(_ongoingTrainingId));
        await _exercisesHistoriesRepository
            .DidNotReceive()
            .AddAsync(Arg.Any<ExerciseHistory>(), Arg.Any<CancellationToken>());
        _ongoingTrainingsRepository.DidNotReceive().Update(Arg.Any<OngoingTraining>());
    }

    [Fact]
    public async Task Handle_WhenAllValidationsPass_ShouldCreateExerciseHistoryWithSkippedStatus()
    {
        // Arrange
        var ongoingTraining = OngoingTrainingFaker.Generate(
            id: _ongoingTrainingId,
            userId: _userId
        );

        // Capture the current exercise ID before the handler runs
        var currentExerciseId = ongoingTraining.CurrentExercise.Id;

        _ongoingTrainingsRepository
            .GetByIdAsync(_ongoingTrainingId, Arg.Any<CancellationToken>())
            .Returns(ongoingTraining);

        _exercisesHistoriesQuery
            .GetByOngoingTrainingIdAsync(_ongoingTrainingId, Arg.Any<CancellationToken>())
            .Returns(new List<ExerciseHistoryReadModel>());

        _exercisesHistoriesRepository
            .GetByOngoingTrainingIdAndExerciseIdAsync(
                _ongoingTrainingId,
                currentExerciseId,
                Arg.Any<CancellationToken>()
            )
            .Returns((ExerciseHistory?)null);

        ExerciseHistory? capturedHistory = null;
        _exercisesHistoriesRepository
            .AddAsync(
                Arg.Do<ExerciseHistory>(eh => capturedHistory = eh),
                Arg.Any<CancellationToken>()
            )
            .Returns(Task.CompletedTask);

        var command = new SkipExerciseCommand(_ongoingTrainingId);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        capturedHistory.Should().NotBeNull();
        capturedHistory!.OngoingTrainingId.Should().Be(_ongoingTrainingId);
        capturedHistory.ExerciseId.Should().Be(currentExerciseId);
        capturedHistory.Status.Should().Be(ExerciseStatus.Skipped);
        capturedHistory.NewExerciseSets.Should().NotBeEmpty();
        _ongoingTrainingsRepository.Received(1).Update(ongoingTraining);
    }

    [Fact]
    public async Task Handle_WhenExerciseHistoryExists_ShouldUpdateExerciseHistoryToSkipped()
    {
        // Arrange
        var ongoingTraining = OngoingTrainingFaker.Generate(
            id: _ongoingTrainingId,
            userId: _userId
        );

        var existingHistory = ExerciseHistoryFaker.GenerateEntity(
            ongoingTrainingId: _ongoingTrainingId,
            exerciseId: ongoingTraining.CurrentExercise.Id
        );

        _ongoingTrainingsRepository
            .GetByIdAsync(_ongoingTrainingId, Arg.Any<CancellationToken>())
            .Returns(ongoingTraining);

        _exercisesHistoriesQuery
            .GetByOngoingTrainingIdAsync(_ongoingTrainingId, Arg.Any<CancellationToken>())
            .Returns(new List<ExerciseHistoryReadModel>());

        _exercisesHistoriesRepository
            .GetByOngoingTrainingIdAndExerciseIdAsync(
                _ongoingTrainingId,
                ongoingTraining.CurrentExercise.Id,
                Arg.Any<CancellationToken>()
            )
            .Returns(existingHistory);

        var command = new SkipExerciseCommand(_ongoingTrainingId);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _exercisesHistoriesRepository
            .Received(1)
            .Update(
                Arg.Is<ExerciseHistory>(eh =>
                    eh.Id == existingHistory.Id && eh.Status == ExerciseStatus.Skipped
                )
            );
        await _exercisesHistoriesRepository
            .DidNotReceive()
            .AddAsync(Arg.Any<ExerciseHistory>(), Arg.Any<CancellationToken>());
        _ongoingTrainingsRepository.Received(1).Update(ongoingTraining);
    }
}
