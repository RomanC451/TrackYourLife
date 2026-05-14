using TrackYourLife.Modules.Trainings.Application.Features.ExercisesHistories.Queries.GetExerciseStats;
using TrackYourLife.Modules.Trainings.Application.UnitTests.Utils;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.ExercisesHistories.Queries.GetExerciseStats;

public class GetExerciseStatsQueryHandlerTests
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IExercisesHistoriesQuery _exercisesHistoriesQuery;
    private readonly IExercisesQuery _exercisesQuery;
    private readonly GetExerciseStatsQueryHandler _handler;
    private readonly UserId _userId;

    public GetExerciseStatsQueryHandlerTests()
    {
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _exercisesHistoriesQuery = Substitute.For<IExercisesHistoriesQuery>();
        _exercisesQuery = Substitute.For<IExercisesQuery>();
        _handler = new GetExerciseStatsQueryHandler(
            _userIdentifierProvider,
            _exercisesHistoriesQuery,
            _exercisesQuery
        );
        _userId = UserId.NewId();
        _userIdentifierProvider.UserId.Returns(_userId);
    }

    [Fact]
    public async Task Handle_WhenExerciseExistsAndHistoriesValid_ShouldReturnFirstVsLastImprovementAndTrends()
    {
        var exerciseId = ExerciseId.NewId();
        var exercise = ExerciseReadModelFaker.Generate(id: exerciseId, name: "Bench Press");
        _exercisesQuery.GetByIdAsync(exerciseId, Arg.Any<CancellationToken>()).Returns(exercise);

        var now = DateTime.UtcNow;
        var historyOne = ExerciseHistoryReadModelFaker.Generate(
            exerciseId: exerciseId,
            createdOnUtc: now.AddDays(-14),
            newExerciseSets:
            [
                ExerciseSet.Create(Guid.NewGuid(), "Set 1", 1, 100, "kg", 5, "reps").Value,
            ]
        );
        var historyTwo = ExerciseHistoryReadModelFaker.Generate(
            exerciseId: exerciseId,
            createdOnUtc: now.AddDays(-7),
            newExerciseSets:
            [
                ExerciseSet.Create(Guid.NewGuid(), "Set 1", 1, 110, "kg", 5, "reps").Value,
            ]
        );

        _exercisesHistoriesQuery
            .GetCompletedByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns([historyOne, historyTwo]);

        var query = new GetExerciseStatsQuery(exerciseId, ExerciseStatsRange.TwelveWeeks);
        var result = await _handler.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.ExerciseId.Should().Be(exerciseId);
        result.Value.IsSupportedExerciseType.Should().BeTrue();
        result.Value.HasEnoughData.Should().BeTrue();
        result.Value.ImprovementTrend.Should().HaveCount(2);
        result.Value.Summary.CompletedSessionsInRange.Should().Be(2);
        result.Value.Summary.SkippedSessionsInRange.Should().Be(0);
        result.Value.Summary.TotalVolumeInRange.Should().BeApproximately(1050, 0.01);
        result.Value.Summary.AverageVolumeInRange.Should().BeApproximately(525, 0.01);
        // (550 - 500) / 500 * 100 = 10%
        result.Value.Summary.ImprovementDeltaPercent.Should().BeApproximately(10, 0.01);
        result.Value.ImprovementTrend[0].Value.Should().BeApproximately(500, 0.01);
        result.Value.ImprovementTrend[1].Value.Should().BeApproximately(550, 0.01);
    }

    [Fact]
    public async Task Handle_WhenCustomDateRange_ShouldFilterHistoriesOutsideWindow()
    {
        var exerciseId = ExerciseId.NewId();
        _exercisesQuery
            .GetByIdAsync(exerciseId, Arg.Any<CancellationToken>())
            .Returns(ExerciseReadModelFaker.Generate(id: exerciseId));

        var inside = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-5));
        var outsideEarly = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-100));

        var hInside = ExerciseHistoryReadModelFaker.Generate(
            exerciseId: exerciseId,
            createdOnUtc: inside.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc),
            newExerciseSets:
            [
                ExerciseSet.Create(Guid.NewGuid(), "Set 1", 1, 50, "kg", 10, "reps").Value,
            ]
        );
        var hOutside = ExerciseHistoryReadModelFaker.Generate(
            exerciseId: exerciseId,
            createdOnUtc: outsideEarly.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc),
            newExerciseSets:
            [
                ExerciseSet.Create(Guid.NewGuid(), "Set 1", 1, 200, "kg", 10, "reps").Value,
            ]
        );

        _exercisesHistoriesQuery
            .GetCompletedByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns([hInside, hOutside]);

        var start = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10));
        var end = DateOnly.FromDateTime(DateTime.UtcNow);
        var query = new GetExerciseStatsQuery(
            exerciseId,
            ExerciseStatsRange.TwelveWeeks,
            start,
            end,
            ExerciseStatsChartMetric.Volume
        );
        var result = await _handler.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.WindowStartDate.Should().Be(start);
        result.Value.WindowEndDate.Should().Be(end);
        result.Value.Summary.CompletedSessionsInRange.Should().Be(1);
        result.Value.Summary.TotalVolumeInRange.Should().BeApproximately(500, 0.01);
        result.Value.HasEnoughData.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WhenMaxWeightMetric_ShouldAggregateDailyMaxAcrossSets()
    {
        var exerciseId = ExerciseId.NewId();
        _exercisesQuery
            .GetByIdAsync(exerciseId, Arg.Any<CancellationToken>())
            .Returns(ExerciseReadModelFaker.Generate(id: exerciseId));

        var day = DateTime.UtcNow.Date;
        var h = ExerciseHistoryReadModelFaker.Generate(
            exerciseId: exerciseId,
            createdOnUtc: day,
            newExerciseSets:
            [
                ExerciseSet.Create(Guid.NewGuid(), "Set 1", 1, 80, "kg", 5, "reps").Value,
                ExerciseSet.Create(Guid.NewGuid(), "Set 2", 2, 100, "kg", 3, "reps").Value,
            ]
        );

        _exercisesHistoriesQuery
            .GetCompletedByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns([h]);

        var query = new GetExerciseStatsQuery(
            exerciseId,
            ExerciseStatsRange.TwelveWeeks,
            null,
            null,
            ExerciseStatsChartMetric.MaxWeight
        );
        var result = await _handler.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.ImprovementTrend.Should().ContainSingle();
        result.Value.ImprovementTrend[0].Value.Should().BeApproximately(100, 0.01);
    }

    [Fact]
    public async Task Handle_WhenSkippedInSameWeek_ShouldStackCounts()
    {
        var exerciseId = ExerciseId.NewId();
        _exercisesQuery
            .GetByIdAsync(exerciseId, Arg.Any<CancellationToken>())
            .Returns(ExerciseReadModelFaker.Generate(id: exerciseId));

        var monday = new DateTime(2026, 5, 11, 12, 0, 0, DateTimeKind.Utc); // Monday
        var completed = ExerciseHistoryReadModelFaker.Generate(
            exerciseId: exerciseId,
            createdOnUtc: monday,
            newExerciseSets:
            [
                ExerciseSet.Create(Guid.NewGuid(), "Set 1", 1, 60, "kg", 5, "reps").Value,
            ],
            status: ExerciseStatus.Completed
        );
        var skipped = ExerciseHistoryReadModelFaker.Generate(
            exerciseId: exerciseId,
            createdOnUtc: monday.AddDays(1),
            newExerciseSets: [],
            status: ExerciseStatus.Skipped
        );

        _exercisesHistoriesQuery
            .GetCompletedByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns([completed, skipped]);

        var query = new GetExerciseStatsQuery(exerciseId, ExerciseStatsRange.TwelveWeeks);
        var result = await _handler.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        var week = result.Value.ConsistencyTrend.Should().ContainSingle().Subject;
        week.CompletedSessionsCount.Should().Be(1);
        week.SkippedSessionsCount.Should().Be(1);
    }

    [Fact]
    public async Task Handle_WhenSetWithoutCount2_ShouldReturnUnsupportedPayload()
    {
        var exerciseId = ExerciseId.NewId();
        _exercisesQuery
            .GetByIdAsync(exerciseId, Arg.Any<CancellationToken>())
            .Returns(ExerciseReadModelFaker.Generate(id: exerciseId));

        var invalidHistory = ExerciseHistoryReadModelFaker.Generate(
            exerciseId: exerciseId,
            newExerciseSets:
            [
                ExerciseSet.Create(Guid.NewGuid(), "Set 1", 1, 60, "seconds").Value,
            ]
        );
        _exercisesHistoriesQuery
            .GetCompletedByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns([invalidHistory]);

        var result = await _handler.Handle(
            new GetExerciseStatsQuery(exerciseId, ExerciseStatsRange.TwelveWeeks),
            default
        );

        result.IsSuccess.Should().BeTrue();
        result.Value.IsSupportedExerciseType.Should().BeFalse();
        result.Value.ImprovementTrend.Should().BeEmpty();
    }
}
