using TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Commands.AdjustExerciseSets;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.OngoingTrainings.Commands.AdjustExerciseSets;

public class AdjustExerciseSetsCommandHandlerTests
{
    private readonly IExercisesQuery _exerciseQuery;
    private readonly IExercisesHistoriesRepository _exercisesHistoriesRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly AdjustExerciseSetsCommandHandler _handler;

    private readonly OngoingTrainingId _ongoingTrainingId;
    private readonly ExerciseId _exerciseId;
    private readonly List<ExerciseSet> _newExerciseSets;

    public AdjustExerciseSetsCommandHandlerTests()
    {
        _exerciseQuery = Substitute.For<IExercisesQuery>();
        _exercisesHistoriesRepository = Substitute.For<IExercisesHistoriesRepository>();
        _dateTimeProvider = Substitute.For<IDateTimeProvider>();
        _handler = new AdjustExerciseSetsCommandHandler(
            _exerciseQuery,
            _exercisesHistoriesRepository,
            _dateTimeProvider
        );

        _ongoingTrainingId = OngoingTrainingId.NewId();
        _exerciseId = ExerciseId.NewId();
        _newExerciseSets =
        [
            ExerciseSet.Create(Guid.NewGuid(), "Set 1", 0, 12, "reps", 55.0f, "kg").Value,
        ];

        _dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);
    }

    [Fact]
    public async Task Handle_WhenExerciseNotFound_ShouldReturnFailure()
    {
        // Arrange
        _exerciseQuery
            .GetByIdAsync(_exerciseId, Arg.Any<CancellationToken>())
            .Returns((ExerciseReadModel?)null);

        var command = new AdjustExerciseSetsCommand(
            _ongoingTrainingId,
            _exerciseId,
            _newExerciseSets
        );

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ExercisesErrors.NotFoundById(_exerciseId));
        await _exercisesHistoriesRepository
            .DidNotReceive()
            .AddAsync(Arg.Any<ExerciseHistory>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenExerciseHistoryCreationFails_ShouldReturnFailure()
    {
        // Arrange
        var exercise = ExerciseReadModelFaker.Generate(id: _exerciseId);

        _exerciseQuery.GetByIdAsync(_exerciseId, Arg.Any<CancellationToken>()).Returns(exercise);
        _exercisesHistoriesRepository
            .GetByOngoingTrainingIdAndExerciseIdAsync(
                _ongoingTrainingId,
                _exerciseId,
                Arg.Any<CancellationToken>()
            )
            .Returns((ExerciseHistory?)null);

        var command = new AdjustExerciseSetsCommand(
            _ongoingTrainingId,
            _exerciseId,
            [] // Empty changes to force creation failure
        );

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        await _exercisesHistoriesRepository
            .DidNotReceive()
            .AddAsync(Arg.Any<ExerciseHistory>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenAllValidationsPass_ShouldCreateExerciseHistory()
    {
        // Arrange
        var exercise = ExerciseReadModelFaker.Generate(id: _exerciseId);

        _exerciseQuery.GetByIdAsync(_exerciseId, Arg.Any<CancellationToken>()).Returns(exercise);
        _exercisesHistoriesRepository
            .GetByOngoingTrainingIdAndExerciseIdAsync(
                _ongoingTrainingId,
                _exerciseId,
                Arg.Any<CancellationToken>()
            )
            .Returns((ExerciseHistory?)null);

        var command = new AdjustExerciseSetsCommand(
            _ongoingTrainingId,
            _exerciseId,
            _newExerciseSets
        );

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _exercisesHistoriesRepository
            .Received(1)
            .AddAsync(
                Arg.Is<ExerciseHistory>(eh =>
                    eh.OngoingTrainingId == _ongoingTrainingId
                    && eh.ExerciseId == _exerciseId
                    && eh.NewExerciseSets.Count == _newExerciseSets.Count
                    && eh.Status == ExerciseStatus.Completed
                    && eh.NewExerciseSets.Zip(_newExerciseSets)
                        .All(pair =>
                            pair.First.Id == pair.Second.Id
                            && Math.Abs(pair.First.Count1 - pair.Second.Count1) < 0.001f
                            && pair.First.Unit1 == pair.Second.Unit1
                            && Math.Abs((pair.First.Count2 ?? 0) - (pair.Second.Count2 ?? 0))
                                < 0.001f
                            && pair.First.Unit2 == pair.Second.Unit2
                        )
                ),
                Arg.Any<CancellationToken>()
            );
        _exercisesHistoriesRepository.DidNotReceive().Update(Arg.Any<ExerciseHistory>());
    }

    [Fact]
    public async Task Handle_WhenExerciseHistoryExists_ShouldUpdateExerciseHistory()
    {
        // Arrange
        var exercise = ExerciseReadModelFaker.Generate(id: _exerciseId);
        var existingHistory = ExerciseHistoryFaker.GenerateEntity(
            ongoingTrainingId: _ongoingTrainingId,
            exerciseId: _exerciseId
        );

        _exerciseQuery.GetByIdAsync(_exerciseId, Arg.Any<CancellationToken>()).Returns(exercise);
        _exercisesHistoriesRepository
            .GetByOngoingTrainingIdAndExerciseIdAsync(
                _ongoingTrainingId,
                _exerciseId,
                Arg.Any<CancellationToken>()
            )
            .Returns(existingHistory);

        var command = new AdjustExerciseSetsCommand(
            _ongoingTrainingId,
            _exerciseId,
            _newExerciseSets
        );

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _exercisesHistoriesRepository
            .Received(1)
            .Update(
                Arg.Is<ExerciseHistory>(eh =>
                    eh.Id == existingHistory.Id
                    && eh.OngoingTrainingId == _ongoingTrainingId
                    && eh.ExerciseId == _exerciseId
                    && eh.Status == ExerciseStatus.Completed
                )
            );
        await _exercisesHistoriesRepository
            .DidNotReceive()
            .AddAsync(Arg.Any<ExerciseHistory>(), Arg.Any<CancellationToken>());
    }
}
