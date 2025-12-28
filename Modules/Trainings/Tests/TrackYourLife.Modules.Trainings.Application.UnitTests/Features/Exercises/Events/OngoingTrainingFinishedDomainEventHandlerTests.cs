using Serilog;
using TrackYourLife.Modules.Trainings.Application.Features.Exercises.Events;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings.Events;
using TrackYourLife.SharedLib.Domain.OutboxMessages;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.Exercises.Events;

public class OngoingTrainingFinishedDomainEventHandlerTests
{
    private readonly IExercisesRepository _exercisesRepository;
    private readonly IExercisesHistoriesRepository _exercisesHistoriesRepository;
    private readonly IOngoingTrainingsQuery _ongoingTrainingsQuery;
    private readonly ILogger _logger;
    private readonly ITrainingsUnitOfWork _trainingsUnitOfWork;
    private readonly OngoingTrainingFinishedDomainEventHandler _handler;

    private readonly OngoingTrainingId _ongoingTrainingId;

    public OngoingTrainingFinishedDomainEventHandlerTests()
    {
        _exercisesRepository = Substitute.For<IExercisesRepository>();
        _exercisesHistoriesRepository = Substitute.For<IExercisesHistoriesRepository>();
        _ongoingTrainingsQuery = Substitute.For<IOngoingTrainingsQuery>();
        _logger = Substitute.For<ILogger>();
        _trainingsUnitOfWork = Substitute.For<ITrainingsUnitOfWork>();
        _handler = new OngoingTrainingFinishedDomainEventHandler(
            _exercisesRepository,
            _exercisesHistoriesRepository,
            _ongoingTrainingsQuery,
            _logger,
            _trainingsUnitOfWork
        );

        _ongoingTrainingId = OngoingTrainingId.NewId();
    }

    [Fact]
    public async Task Handle_WhenOngoingTrainingNotFound_ShouldThrowEventFailedException()
    {
        // Arrange
        _ongoingTrainingsQuery
            .GetByIdAsync(_ongoingTrainingId, Arg.Any<CancellationToken>())
            .Returns((OngoingTrainingReadModel?)null);

        var domainEvent = new OngoingTrainingFinishedDomainEvent(_ongoingTrainingId);

        // Act & Assert
        await Assert.ThrowsAsync<EventFailedException>(() => _handler.Handle(domainEvent, default));
    }

    [Fact]
    public async Task Handle_WhenNoExerciseHistories_ShouldCompleteSuccessfully()
    {
        // Arrange
        var ongoingTraining = OngoingTrainingReadModelFaker.Generate(id: _ongoingTrainingId);

        _ongoingTrainingsQuery
            .GetByIdAsync(_ongoingTrainingId, Arg.Any<CancellationToken>())
            .Returns(ongoingTraining);
        _exercisesHistoriesRepository
            .GetByOngoingTrainingIdAndAreNotAppliedAsync(
                _ongoingTrainingId,
                Arg.Any<CancellationToken>()
            )
            .Returns([]);

        var domainEvent = new OngoingTrainingFinishedDomainEvent(_ongoingTrainingId);

        // Act
        await _handler.Handle(domainEvent, default);

        // Assert
        await _trainingsUnitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        _exercisesRepository.DidNotReceive().Update(Arg.Any<Exercise>());
        _exercisesHistoriesRepository.DidNotReceive().Update(Arg.Any<ExerciseHistory>());
    }

    [Fact]
    public async Task Handle_WhenExerciseNotFound_ShouldLogErrorAndContinue()
    {
        // Arrange
        var ongoingTraining = OngoingTrainingReadModelFaker.Generate(id: _ongoingTrainingId);
        var exerciseHistory = ExerciseHistoryFaker.GenerateEntity(
            ongoingTrainingId: _ongoingTrainingId
        );
        var exerciseHistories = new List<ExerciseHistory> { exerciseHistory };

        _ongoingTrainingsQuery
            .GetByIdAsync(_ongoingTrainingId, Arg.Any<CancellationToken>())
            .Returns(ongoingTraining);
        _exercisesHistoriesRepository
            .GetByOngoingTrainingIdAndAreNotAppliedAsync(
                _ongoingTrainingId,
                Arg.Any<CancellationToken>()
            )
            .Returns(exerciseHistories);
        _exercisesRepository
            .GetByIdAsync(exerciseHistory.ExerciseId, Arg.Any<CancellationToken>())
            .Returns((Exercise?)null);

        var domainEvent = new OngoingTrainingFinishedDomainEvent(_ongoingTrainingId);

        // Act
        await _handler.Handle(domainEvent, default);

        // Assert
        _logger
            .Received(1)
            .Error("Exercise with id {ExerciseId} not found", exerciseHistory.ExerciseId);
        await _trainingsUnitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenAllValidationsPass_ShouldUpdateExercisesAndHistories()
    {
        // Arrange
        var ongoingTraining = OngoingTrainingReadModelFaker.Generate(id: _ongoingTrainingId);
        var exercise = ExerciseFaker.Generate();
        var exerciseSet = exercise.ExerciseSets[0];
        var newExerciseSet = ExerciseSet
            .Create(
                exerciseSet.Id,
                exerciseSet.Name,
                exerciseSet.OrderIndex,
                exerciseSet.Count1 + 2,
                exerciseSet.Unit1,
                (exerciseSet.Count2 ?? 0) + 5,
                exerciseSet.Unit2
            )
            .Value;
        var exerciseHistory = ExerciseHistoryFaker.GenerateEntity(
            ongoingTrainingId: _ongoingTrainingId,
            exerciseId: exercise.Id,
            newExerciseSets: [newExerciseSet]
        );
        var exerciseHistories = new List<ExerciseHistory> { exerciseHistory };

        _ongoingTrainingsQuery
            .GetByIdAsync(_ongoingTrainingId, Arg.Any<CancellationToken>())
            .Returns(ongoingTraining);
        _exercisesHistoriesRepository
            .GetByOngoingTrainingIdAndAreNotAppliedAsync(
                _ongoingTrainingId,
                Arg.Any<CancellationToken>()
            )
            .Returns(exerciseHistories);
        _exercisesRepository
            .GetByIdAsync(exerciseHistory.ExerciseId, Arg.Any<CancellationToken>())
            .Returns(exercise);

        var domainEvent = new OngoingTrainingFinishedDomainEvent(_ongoingTrainingId);

        // Act
        await _handler.Handle(domainEvent, default);

        // Assert
        _exercisesRepository.Received(1).Update(exercise);
        _exercisesHistoriesRepository.Received(1).Update(exerciseHistory);
        await _trainingsUnitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
