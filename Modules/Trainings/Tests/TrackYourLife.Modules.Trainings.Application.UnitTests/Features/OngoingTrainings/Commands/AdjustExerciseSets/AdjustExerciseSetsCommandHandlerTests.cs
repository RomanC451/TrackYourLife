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
    private readonly List<ExerciseSetChange> _exerciseSetChanges;

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
        _exerciseSetChanges = [ExerciseSetChangeFaker.Generate()];

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
            _exerciseSetChanges
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

        var command = new AdjustExerciseSetsCommand(
            _ongoingTrainingId,
            _exerciseId,
            _exerciseSetChanges
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
                    && eh.ExerciseSetChanges.Count == _exerciseSetChanges.Count
                    && eh.ExerciseSetChanges.Zip(_exerciseSetChanges)
                        .All(pair =>
                            pair.First.SetId == pair.Second.SetId
                            && Math.Abs(pair.First.WeightChange - pair.Second.WeightChange) < 0.001f
                            && pair.First.RepsChange == pair.Second.RepsChange
                        )
                ),
                Arg.Any<CancellationToken>()
            );
    }
}
