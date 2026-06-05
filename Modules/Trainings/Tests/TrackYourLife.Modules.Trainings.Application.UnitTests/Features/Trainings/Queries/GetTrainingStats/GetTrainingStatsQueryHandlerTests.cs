using TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetTrainingStats;
using TrackYourLife.Modules.Trainings.Application.UnitTests.Utils;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.Trainings.Queries.GetTrainingStats;

public class GetTrainingStatsQueryHandlerTests
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly ITrainingsQuery _trainingsQuery;
    private readonly IOngoingTrainingsQuery _ongoingTrainingsQuery;
    private readonly IExercisesHistoriesQuery _exercisesHistoriesQuery;
    private readonly GetTrainingStatsQueryHandler _handler;

    private readonly UserId _userId;
    private readonly TrainingId _trainingId;

    public GetTrainingStatsQueryHandlerTests()
    {
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _trainingsQuery = Substitute.For<ITrainingsQuery>();
        _ongoingTrainingsQuery = Substitute.For<IOngoingTrainingsQuery>();
        _exercisesHistoriesQuery = Substitute.For<IExercisesHistoriesQuery>();
        _handler = new GetTrainingStatsQueryHandler(
            _userIdentifierProvider,
            _trainingsQuery,
            _ongoingTrainingsQuery,
            _exercisesHistoriesQuery
        );

        _userId = UserId.NewId();
        _trainingId = TrainingId.NewId();
        _userIdentifierProvider.UserId.Returns(_userId);
    }

    [Fact]
    public async Task Handle_WhenTrainingNotFound_ShouldReturnFailure()
    {
        _trainingsQuery
            .GetByIdAsync(_trainingId, Arg.Any<CancellationToken>())
            .Returns((TrainingReadModel?)null);

        var result = await _handler.Handle(
            new GetTrainingStatsQuery(_trainingId, ExerciseStatsRange.TwelveWeeks),
            default
        );

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TrainingsErrors.NotFoundById(_trainingId));
    }

    [Fact]
    public async Task Handle_WhenTrainingNotOwned_ShouldReturnFailure()
    {
        var training = TrainingReadModelFaker.Generate(id: _trainingId, userId: UserId.NewId());
        _trainingsQuery
            .GetByIdAsync(_trainingId, Arg.Any<CancellationToken>())
            .Returns(training);

        var result = await _handler.Handle(
            new GetTrainingStatsQuery(_trainingId, ExerciseStatsRange.TwelveWeeks),
            default
        );

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TrainingsErrors.NotOwned(_trainingId));
    }

    [Fact]
    public async Task Handle_WhenNoCompletedSessions_ShouldReturnEmptyStatsWithTemplateMetadata()
    {
        var training = TrainingReadModelFaker.Generate(
            id: _trainingId,
            userId: _userId,
            name: "Push Day",
            duration: 45
        );
        _trainingsQuery
            .GetByIdAsync(_trainingId, Arg.Any<CancellationToken>())
            .Returns(training);
        _ongoingTrainingsQuery
            .GetCompletedByUserIdAndTrainingIdAsync(
                _userId,
                _trainingId,
                Arg.Any<CancellationToken>()
            )
            .Returns(Array.Empty<OngoingTrainingReadModel>());
        _exercisesHistoriesQuery
            .GetByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(Array.Empty<ExerciseHistoryReadModel>());

        var result = await _handler.Handle(
            new GetTrainingStatsQuery(_trainingId, ExerciseStatsRange.TwelveWeeks),
            default
        );

        result.IsSuccess.Should().BeTrue();
        result.Value.TrainingId.Should().Be(_trainingId);
        result.Value.TrainingName.Should().Be("Push Day");
        result.Value.EstimatedDurationSeconds.Should().Be(45 * 60);
        result.Value.Summary.SessionsCompleted.Should().Be(0);
        result.Value.RecentSessions.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WhenSessionsExist_ShouldReturnSummaryAndCapRecentSessionsAtFive()
    {
        var training = TrainingReadModelFaker.Generate(id: _trainingId, userId: _userId);
        var now = DateTime.UtcNow;

        var sessions = Enumerable
            .Range(0, 7)
            .Select(i =>
            {
                var finished = now.AddDays(-i);
                var session = OngoingTrainingReadModelFaker.Generate(
                    userId: _userId,
                    startedOnUtc: finished.AddMinutes(-60),
                    finishedOnUtc: finished,
                    caloriesBurned: 200 + i,
                    training: training
                );
                return session;
            })
            .ToList();

        _trainingsQuery
            .GetByIdAsync(_trainingId, Arg.Any<CancellationToken>())
            .Returns(training);
        _ongoingTrainingsQuery
            .GetCompletedByUserIdAndTrainingIdAsync(
                _userId,
                _trainingId,
                Arg.Any<CancellationToken>()
            )
            .Returns(sessions);
        _exercisesHistoriesQuery
            .GetByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(Array.Empty<ExerciseHistoryReadModel>());

        var result = await _handler.Handle(
            new GetTrainingStatsQuery(_trainingId, ExerciseStatsRange.TwelveWeeks),
            default
        );

        result.IsSuccess.Should().BeTrue();
        result.Value.Summary.SessionsCompleted.Should().Be(7);
        result.Value.Summary.FullyCompletedCount.Should().Be(7);
        result.Value.Summary.CompletionRate.Should().Be(100);
        result.Value.RecentSessions.Should().HaveCount(5);
        result.Value.RecentSessions[0].FinishedOnUtc.Should().BeAfter(
            result.Value.RecentSessions[1].FinishedOnUtc
        );
    }

    [Fact]
    public async Task Handle_WhenSessionHasSkippedExercise_ShouldReduceCompletionRate()
    {
        var training = TrainingReadModelFaker.Generate(id: _trainingId, userId: _userId);
        var finished = DateTime.UtcNow.AddDays(-1);
        var ongoingId = OngoingTrainingId.NewId();
        var session = OngoingTrainingReadModelFaker.Generate(
            id: ongoingId,
            userId: _userId,
            startedOnUtc: finished.AddMinutes(-45),
            finishedOnUtc: finished,
            training: training
        );

        _trainingsQuery
            .GetByIdAsync(_trainingId, Arg.Any<CancellationToken>())
            .Returns(training);
        _ongoingTrainingsQuery
            .GetCompletedByUserIdAndTrainingIdAsync(
                _userId,
                _trainingId,
                Arg.Any<CancellationToken>()
            )
            .Returns(new[] { session });
        _exercisesHistoriesQuery
            .GetByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(
                new[]
                {
                    ExerciseHistoryReadModelFaker.Generate(
                        ongoingTrainingId: ongoingId,
                        status: ExerciseStatus.Skipped
                    ),
                }
            );

        var result = await _handler.Handle(
            new GetTrainingStatsQuery(_trainingId, ExerciseStatsRange.TwelveWeeks),
            default
        );

        result.IsSuccess.Should().BeTrue();
        result.Value.Summary.SessionsCompleted.Should().Be(1);
        result.Value.Summary.FullyCompletedCount.Should().Be(0);
        result.Value.Summary.WithSkippedCount.Should().Be(1);
        result.Value.Summary.CompletionRate.Should().Be(0);
    }

    [Fact]
    public async Task Handle_WhenCustomDateRangeProvided_ShouldExcludeSessionsOutsideWindow()
    {
        var training = TrainingReadModelFaker.Generate(id: _trainingId, userId: _userId);
        var inRangeFinished = DateTime.UtcNow.AddDays(-3);
        var outOfRangeFinished = DateTime.UtcNow.AddDays(-40);

        var inRangeSession = OngoingTrainingReadModelFaker.Generate(
            userId: _userId,
            startedOnUtc: inRangeFinished.AddMinutes(-30),
            finishedOnUtc: inRangeFinished,
            training: training
        );
        var outOfRangeSession = OngoingTrainingReadModelFaker.Generate(
            userId: _userId,
            startedOnUtc: outOfRangeFinished.AddMinutes(-30),
            finishedOnUtc: outOfRangeFinished,
            training: training
        );

        _trainingsQuery
            .GetByIdAsync(_trainingId, Arg.Any<CancellationToken>())
            .Returns(training);
        _ongoingTrainingsQuery
            .GetCompletedByUserIdAndTrainingIdAsync(
                _userId,
                _trainingId,
                Arg.Any<CancellationToken>()
            )
            .Returns(new[] { inRangeSession, outOfRangeSession });
        _exercisesHistoriesQuery
            .GetByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(Array.Empty<ExerciseHistoryReadModel>());

        var startDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7));
        var endDate = DateOnly.FromDateTime(DateTime.UtcNow);

        var result = await _handler.Handle(
            new GetTrainingStatsQuery(
                _trainingId,
                ExerciseStatsRange.TwelveWeeks,
                startDate,
                endDate
            ),
            default
        );

        result.IsSuccess.Should().BeTrue();
        result.Value.Summary.SessionsCompleted.Should().Be(1);
        result.Value.Summary.WindowStartDate.Should().Be(startDate);
        result.Value.Summary.WindowEndDate.Should().Be(endDate);
    }

    [Fact]
    public async Task Handle_WhenChartAggregationIsAverage_ShouldAverageDurationWithinWeek()
    {
        var training = TrainingReadModelFaker.Generate(id: _trainingId, userId: _userId);
        var weekStart = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10));
        var day1 = weekStart.AddDays(1);
        var day2 = weekStart.AddDays(3);

        var session1 = OngoingTrainingReadModelFaker.Generate(
            userId: _userId,
            startedOnUtc: day1.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc),
            finishedOnUtc: day1.ToDateTime(new TimeOnly(1, 0), DateTimeKind.Utc),
            training: training
        );
        var session2 = OngoingTrainingReadModelFaker.Generate(
            userId: _userId,
            startedOnUtc: day2.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc),
            finishedOnUtc: day2.ToDateTime(new TimeOnly(2, 0), DateTimeKind.Utc),
            training: training
        );

        _trainingsQuery
            .GetByIdAsync(_trainingId, Arg.Any<CancellationToken>())
            .Returns(training);
        _ongoingTrainingsQuery
            .GetCompletedByUserIdAndTrainingIdAsync(
                _userId,
                _trainingId,
                Arg.Any<CancellationToken>()
            )
            .Returns(new[] { session1, session2 });
        _exercisesHistoriesQuery
            .GetByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(Array.Empty<ExerciseHistoryReadModel>());

        var result = await _handler.Handle(
            new GetTrainingStatsQuery(
                _trainingId,
                ExerciseStatsRange.TwelveWeeks,
                ChartAggregationType: AggregationType.Average
            ),
            default
        );

        result.IsSuccess.Should().BeTrue();
        result.Value.ChartAggregationType.Should().Be(AggregationType.Average);
        var weekBucket = result.Value.DurationTrend.FirstOrDefault(t => t.Value > 0);
        weekBucket.Should().NotBeNull();
        weekBucket!.Value.Should().BeApproximately(5400, 1);
    }
}
