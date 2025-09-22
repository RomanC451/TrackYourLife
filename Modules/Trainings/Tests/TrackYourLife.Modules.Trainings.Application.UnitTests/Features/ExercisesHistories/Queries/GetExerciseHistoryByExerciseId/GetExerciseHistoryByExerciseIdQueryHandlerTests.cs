using TrackYourLife.Modules.Trainings.Application.Features.ExercisesHistories.Queries.GetExerciseHistoryByExerciseId;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.ExercisesHistories.Queries.GetExerciseHistoryByExerciseId;

public class GetExerciseHistoryByExerciseIdQueryHandlerTests
{
    private readonly IExercisesQuery _exercisesQuery;
    private readonly IExercisesHistoriesQuery _exercisesHistoriesQuery;
    private readonly GetExerciseHistoryByExerciseIdQueryHandler _handler;

    private readonly ExerciseId _exerciseId;

    public GetExerciseHistoryByExerciseIdQueryHandlerTests()
    {
        _exercisesQuery = Substitute.For<IExercisesQuery>();
        _exercisesHistoriesQuery = Substitute.For<IExercisesHistoriesQuery>();
        _handler = new GetExerciseHistoryByExerciseIdQueryHandler(
            _exercisesQuery,
            _exercisesHistoriesQuery
        );

        _exerciseId = ExerciseId.NewId();
    }

    [Fact]
    public async Task Handle_WhenNoExerciseHistoriesFound_ShouldReturnEmptyCollection()
    {
        // Arrange
        var exercise = ExerciseReadModelFaker.Generate(_exerciseId);

        _exercisesQuery.GetByIdAsync(_exerciseId, Arg.Any<CancellationToken>()).Returns(exercise);

        _exercisesHistoriesQuery
            .GetByExerciseIdAsync(_exerciseId, Arg.Any<CancellationToken>())
            .Returns([]);

        var query = new GetExerciseHistoryByExerciseIdQuery(_exerciseId);

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WhenExerciseHistoriesFound_ShouldReturnExerciseHistories()
    {
        // Arrange
        var exercise = ExerciseReadModelFaker.Generate(_exerciseId);
        var exerciseHistories = new List<ExerciseHistoryReadModel>
        {
            ExerciseHistoryReadModelFaker.Generate(),
            ExerciseHistoryReadModelFaker.Generate(),
        };

        _exercisesQuery.GetByIdAsync(_exerciseId, Arg.Any<CancellationToken>()).Returns(exercise);

        _exercisesHistoriesQuery
            .GetByExerciseIdAsync(_exerciseId, Arg.Any<CancellationToken>())
            .Returns(exerciseHistories);

        var query = new GetExerciseHistoryByExerciseIdQuery(_exerciseId);

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(exerciseHistories);
    }
}
