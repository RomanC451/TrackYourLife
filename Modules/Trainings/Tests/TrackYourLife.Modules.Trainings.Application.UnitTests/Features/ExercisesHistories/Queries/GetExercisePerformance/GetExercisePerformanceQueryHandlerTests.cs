using TrackYourLife.Modules.Trainings.Application.Features.ExercisesHistories.Queries.GetExercisePerformance;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.ExercisesHistories.Queries.GetExercisePerformance;

public class GetExercisePerformanceQueryHandlerTests
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IExercisesHistoriesQuery _exercisesHistoriesQuery;
    private readonly IExercisesQuery _exercisesQuery;
    private readonly GetExercisePerformanceQueryHandler _handler;

    private readonly UserId _userId;

    public GetExercisePerformanceQueryHandlerTests()
    {
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _exercisesHistoriesQuery = Substitute.For<IExercisesHistoriesQuery>();
        _exercisesQuery = Substitute.For<IExercisesQuery>();
        _handler = new GetExercisePerformanceQueryHandler(
            _userIdentifierProvider,
            _exercisesHistoriesQuery,
            _exercisesQuery
        );

        _userId = UserId.NewId();
        _userIdentifierProvider.UserId.Returns(_userId);
    }

    [Fact]
    public async Task Handle_WhenNoHistories_ShouldReturnEmptyPagedList()
    {
        _exercisesHistoriesQuery
            .GetByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(Array.Empty<ExerciseHistoryReadModel>());

        var query = new GetExercisePerformanceQuery(
            StartDate: null,
            EndDate: null,
            ExerciseId: null,
            PerformanceCalculationMethod.Sequential,
            Page: 1,
            PageSize: 10
        );

        var result = await _handler.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WhenNoDateRange_ShouldUseGetByUserIdAsync()
    {
        _exercisesHistoriesQuery
            .GetByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(Array.Empty<ExerciseHistoryReadModel>());

        var query = new GetExercisePerformanceQuery(
            StartDate: null,
            EndDate: null,
            ExerciseId: null,
            PerformanceCalculationMethod.Sequential,
            Page: 1,
            PageSize: 10
        );

        await _handler.Handle(query, default);

        await _exercisesHistoriesQuery
            .Received(1)
            .GetByUserIdAsync(_userId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenDateRangeProvided_ShouldUseGetByUserIdAndDateRangeAsync()
    {
        var startDate = DateTime.UtcNow.AddDays(-7);
        var endDate = DateTime.UtcNow;
        _exercisesHistoriesQuery
            .GetByUserIdAndDateRangeAsync(
                _userId,
                Arg.Any<DateTime>(),
                Arg.Any<DateTime>(),
                Arg.Any<CancellationToken>()
            )
            .Returns(Array.Empty<ExerciseHistoryReadModel>());

        var query = new GetExercisePerformanceQuery(
            StartDate: startDate,
            EndDate: endDate,
            ExerciseId: null,
            PerformanceCalculationMethod.Sequential,
            Page: 1,
            PageSize: 10
        );

        await _handler.Handle(query, default);

        await _exercisesHistoriesQuery
            .Received(1)
            .GetByUserIdAndDateRangeAsync(
                _userId,
                DateTime.SpecifyKind(startDate, DateTimeKind.Utc),
                DateTime.SpecifyKind(endDate, DateTimeKind.Utc),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task Handle_WhenExerciseIdProvided_ShouldFilterByExerciseId()
    {
        var exerciseId = ExerciseId.NewId();
        _exercisesHistoriesQuery
            .GetByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(Array.Empty<ExerciseHistoryReadModel>());

        var query = new GetExercisePerformanceQuery(
            StartDate: null,
            EndDate: null,
            ExerciseId: exerciseId,
            PerformanceCalculationMethod.Sequential,
            Page: 1,
            PageSize: 10
        );

        await _handler.Handle(query, default);

        await _exercisesHistoriesQuery
            .Received(1)
            .GetByUserIdAsync(_userId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenLessThanTwoCompletedHistories_ShouldReturnZeroImprovement()
    {
        var exerciseId = ExerciseId.NewId();
        var exerciseName = "Bench Press";
        var setVolume100 = new List<ExerciseSet>
        {
            ExerciseSet.Create(Guid.NewGuid(), "Set 1", 0, 100, "reps").Value!,
        };
        var history = ExerciseHistoryReadModelFaker.Generate(
            exerciseId: exerciseId,
            newExerciseSets: setVolume100,
            oldExerciseSets: setVolume100
        );

        _exercisesHistoriesQuery
            .GetByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(new[] { history });
        _exercisesQuery
            .GetEnumerableWithinIdsCollectionAsync(
                Arg.Is<IEnumerable<ExerciseId>>(ids => ids.Contains(exerciseId)),
                Arg.Any<CancellationToken>()
            )
            .Returns(new[] { ExerciseReadModelFaker.Generate(id: exerciseId, name: exerciseName) });

        var query = new GetExercisePerformanceQuery(
            StartDate: null,
            EndDate: null,
            ExerciseId: null,
            PerformanceCalculationMethod.Sequential,
            Page: 1,
            PageSize: 10
        );

        var result = await _handler.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().HaveCount(1);
        result.Value.Items.First().ExerciseId.Should().Be(exerciseId);
        result.Value.Items.First().ExerciseName.Should().Be(exerciseName);
        result.Value.Items.First().ImprovementPercentage.Should().Be(0.0f);
    }

    [Fact]
    public async Task Handle_WhenTwoCompletedHistories_SequentialMethod_ShouldCalculateImprovement()
    {
        var exerciseId = ExerciseId.NewId();
        var exerciseName = "Squat";
        var setVol100 = new List<ExerciseSet>
        {
            ExerciseSet.Create(Guid.NewGuid(), "Set 1", 0, 100, "reps").Value!,
        };
        var setVol120 = new List<ExerciseSet>
        {
            ExerciseSet.Create(Guid.NewGuid(), "Set 1", 0, 120, "reps").Value!,
        };
        var created1 = DateTime.UtcNow.AddDays(-2);
        var created2 = DateTime.UtcNow.AddDays(-1);
        var histories = new List<ExerciseHistoryReadModel>
        {
            ExerciseHistoryReadModelFaker.Generate(
                exerciseId: exerciseId,
                newExerciseSets: setVol100,
                oldExerciseSets: setVol100,
                createdOnUtc: created1
            ),
            ExerciseHistoryReadModelFaker.Generate(
                exerciseId: exerciseId,
                newExerciseSets: setVol120,
                oldExerciseSets: setVol120,
                createdOnUtc: created2
            ),
        };

        _exercisesHistoriesQuery
            .GetByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(histories);
        _exercisesQuery
            .GetEnumerableWithinIdsCollectionAsync(
                Arg.Is<IEnumerable<ExerciseId>>(ids => ids.Contains(exerciseId)),
                Arg.Any<CancellationToken>()
            )
            .Returns(new[] { ExerciseReadModelFaker.Generate(id: exerciseId, name: exerciseName) });

        var query = new GetExercisePerformanceQuery(
            StartDate: null,
            EndDate: null,
            ExerciseId: null,
            PerformanceCalculationMethod.Sequential,
            Page: 1,
            PageSize: 10
        );

        var result = await _handler.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().HaveCount(1);
        result.Value.Items.First().ImprovementPercentage.Should().BeApproximately(20.0f, 0.1f); // (120-100)/100 * 100 = 20%
    }

    [Fact]
    public async Task Handle_WhenTwoCompletedHistories_FirstVsLastMethod_ShouldCalculateImprovement()
    {
        var exerciseId = ExerciseId.NewId();
        var exerciseName = "Deadlift";
        var setVol100 = new List<ExerciseSet>
        {
            ExerciseSet.Create(Guid.NewGuid(), "Set 1", 0, 100, "reps").Value!,
        };
        var setVol150 = new List<ExerciseSet>
        {
            ExerciseSet.Create(Guid.NewGuid(), "Set 1", 0, 150, "reps").Value!,
        };
        var histories = new List<ExerciseHistoryReadModel>
        {
            ExerciseHistoryReadModelFaker.Generate(
                exerciseId: exerciseId,
                newExerciseSets: setVol100,
                oldExerciseSets: setVol100,
                createdOnUtc: DateTime.UtcNow.AddDays(-2)
            ),
            ExerciseHistoryReadModelFaker.Generate(
                exerciseId: exerciseId,
                newExerciseSets: setVol150,
                oldExerciseSets: setVol150,
                createdOnUtc: DateTime.UtcNow.AddDays(-1)
            ),
        };

        _exercisesHistoriesQuery
            .GetByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(histories);
        _exercisesQuery
            .GetEnumerableWithinIdsCollectionAsync(
                Arg.Is<IEnumerable<ExerciseId>>(ids => ids.Contains(exerciseId)),
                Arg.Any<CancellationToken>()
            )
            .Returns(new[] { ExerciseReadModelFaker.Generate(id: exerciseId, name: exerciseName) });

        var query = new GetExercisePerformanceQuery(
            StartDate: null,
            EndDate: null,
            ExerciseId: null,
            PerformanceCalculationMethod.FirstVsLast,
            Page: 1,
            PageSize: 10
        );

        var result = await _handler.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().HaveCount(1);
        result.Value.Items.First().ImprovementPercentage.Should().BeApproximately(50.0f, 0.1f); // (150-100)/100 * 100 = 50%
    }

    [Fact]
    public async Task Handle_WhenPageOutOfRange_ShouldReturnFailure()
    {
        var exerciseId = ExerciseId.NewId();
        var setVol100 = new List<ExerciseSet>
        {
            ExerciseSet.Create(Guid.NewGuid(), "Set 1", 0, 100, "reps").Value!,
        };
        var setVol120 = new List<ExerciseSet>
        {
            ExerciseSet.Create(Guid.NewGuid(), "Set 1", 0, 120, "reps").Value!,
        };
        var histories = new List<ExerciseHistoryReadModel>
        {
            ExerciseHistoryReadModelFaker.Generate(
                exerciseId: exerciseId,
                newExerciseSets: setVol100,
                oldExerciseSets: setVol100
            ),
            ExerciseHistoryReadModelFaker.Generate(
                exerciseId: exerciseId,
                newExerciseSets: setVol120,
                oldExerciseSets: setVol120
            ),
        };

        _exercisesHistoriesQuery
            .GetByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(histories);
        _exercisesQuery
            .GetEnumerableWithinIdsCollectionAsync(
                Arg.Any<IEnumerable<ExerciseId>>(),
                Arg.Any<CancellationToken>()
            )
            .Returns(new[] { ExerciseReadModelFaker.Generate(id: exerciseId) });

        var query = new GetExercisePerformanceQuery(
            StartDate: null,
            EndDate: null,
            ExerciseId: null,
            PerformanceCalculationMethod.Sequential,
            Page: 2,
            PageSize: 10
        );

        var result = await _handler.Handle(query, default);

        result.IsFailure.Should().BeTrue();
    }
}
