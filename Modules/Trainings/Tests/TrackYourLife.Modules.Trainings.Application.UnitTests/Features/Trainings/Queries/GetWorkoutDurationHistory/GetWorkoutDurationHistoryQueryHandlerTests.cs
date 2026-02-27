using TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetWorkoutDurationHistory;
using TrackYourLife.Modules.Trainings.Application.UnitTests.Utils;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.Trainings.Queries.GetWorkoutDurationHistory;

public class GetWorkoutDurationHistoryQueryHandlerTests
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IOngoingTrainingsQuery _ongoingTrainingsQuery;
    private readonly GetWorkoutDurationHistoryQueryHandler _handler;

    private readonly UserId _userId;

    public GetWorkoutDurationHistoryQueryHandlerTests()
    {
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _ongoingTrainingsQuery = Substitute.For<IOngoingTrainingsQuery>();
        _handler = new GetWorkoutDurationHistoryQueryHandler(
            _userIdentifierProvider,
            _ongoingTrainingsQuery
        );

        _userId = UserId.NewId();
        _userIdentifierProvider.UserId.Returns(_userId);
    }

    [Fact]
    public async Task Handle_WhenNoCompletedWorkouts_ShouldReturnEmptyList()
    {
        _ongoingTrainingsQuery
            .GetCompletedByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(Array.Empty<OngoingTrainingReadModel>());

        var query = new GetWorkoutDurationHistoryQuery(
            StartDate: null,
            EndDate: null,
            OverviewType.Daily,
            AggregationType.Sum
        );

        var result = await _handler.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WhenDateRangeProvided_ShouldCallGetCompletedByUserIdAndDateRangeAsync()
    {
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7));
        var endDate = DateOnly.FromDateTime(DateTime.UtcNow);
        _ongoingTrainingsQuery
            .GetCompletedByUserIdAndDateRangeAsync(
                _userId,
                Arg.Any<DateOnly>(),
                Arg.Any<DateOnly>(),
                Arg.Any<CancellationToken>()
            )
            .Returns(Array.Empty<OngoingTrainingReadModel>());

        var query = new GetWorkoutDurationHistoryQuery(
            StartDate: startDate,
            EndDate: endDate,
            OverviewType.Daily,
            AggregationType.Sum
        );

        await _handler.Handle(query, default);

        await _ongoingTrainingsQuery
            .Received(1)
            .GetCompletedByUserIdAndDateRangeAsync(
                _userId,
                startDate,
                endDate,
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task Handle_ShouldUseCurrentUserId()
    {
        _ongoingTrainingsQuery
            .GetCompletedByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(Array.Empty<OngoingTrainingReadModel>());

        var query = new GetWorkoutDurationHistoryQuery(
            StartDate: null,
            EndDate: null,
            OverviewType.Daily,
            AggregationType.Sum
        );

        await _handler.Handle(query, default);

        await _ongoingTrainingsQuery
            .Received(1)
            .GetCompletedByUserIdAsync(_userId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenNoDateRangeAndCompletedWorkoutsExist_ShouldReturnDurationHistory()
    {
        var started = DateTime.UtcNow.AddDays(-2);
        var finished = started.AddMinutes(45);
        var completed = OngoingTrainingReadModelFaker.Generate(
            userId: _userId,
            startedOnUtc: started,
            finishedOnUtc: finished
        );

        _ongoingTrainingsQuery
            .GetCompletedByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(new[] { completed });

        var query = new GetWorkoutDurationHistoryQuery(
            StartDate: null,
            EndDate: null,
            OverviewType.Daily,
            AggregationType.Sum
        );

        var result = await _handler.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        var day = result.Value[0];
        day.Value.Should().Be(45 * 60); // 45 minutes in seconds
    }

    [Fact]
    public async Task Handle_WhenOverviewTypeWeekly_ShouldReturnWeeklyAggregatedDuration()
    {
        var startDateDt = DateTime.UtcNow.AddDays(-14);
        var endDateDt = DateTime.UtcNow;
        var startDate = DateOnly.FromDateTime(startDateDt);
        var endDate = DateOnly.FromDateTime(endDateDt);
        var completed = OngoingTrainingReadModelFaker.Generate(
            userId: _userId,
            startedOnUtc: startDateDt,
            finishedOnUtc: startDateDt.AddMinutes(30)
        );

        _ongoingTrainingsQuery
            .GetCompletedByUserIdAndDateRangeAsync(
                _userId,
                Arg.Any<DateOnly>(),
                Arg.Any<DateOnly>(),
                Arg.Any<CancellationToken>()
            )
            .Returns(new[] { completed });

        var query = new GetWorkoutDurationHistoryQuery(
            StartDate: startDate,
            EndDate: endDate,
            OverviewType.Weekly,
            AggregationType.Sum
        );

        var result = await _handler.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Handle_WhenOverviewTypeMonthly_ShouldReturnMonthlyAggregatedDuration()
    {
        var startDateDt = DateTime.UtcNow.AddMonths(-2);
        var endDateDt = DateTime.UtcNow;
        var startDate = DateOnly.FromDateTime(startDateDt);
        var endDate = DateOnly.FromDateTime(endDateDt);
        var completed = OngoingTrainingReadModelFaker.Generate(
            userId: _userId,
            startedOnUtc: startDateDt,
            finishedOnUtc: startDateDt.AddMinutes(45)
        );

        _ongoingTrainingsQuery
            .GetCompletedByUserIdAndDateRangeAsync(
                _userId,
                Arg.Any<DateOnly>(),
                Arg.Any<DateOnly>(),
                Arg.Any<CancellationToken>()
            )
            .Returns(new[] { completed });

        var query = new GetWorkoutDurationHistoryQuery(
            StartDate: startDate,
            EndDate: endDate,
            OverviewType.Monthly,
            AggregationType.Sum
        );

        var result = await _handler.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Handle_WhenAggregationTypeAverage_ShouldReturnAverageDuration()
    {
        var startDateDt = DateTime.UtcNow.AddDays(-1);
        var endDateDt = DateTime.UtcNow;
        var startDate = DateOnly.FromDateTime(startDateDt);
        var endDate = DateOnly.FromDateTime(endDateDt);
        var completed1 = OngoingTrainingReadModelFaker.Generate(
            userId: _userId,
            startedOnUtc: startDateDt,
            finishedOnUtc: startDateDt.AddMinutes(30)
        );
        var completed2 = OngoingTrainingReadModelFaker.Generate(
            userId: _userId,
            startedOnUtc: startDateDt.AddHours(2),
            finishedOnUtc: startDateDt.AddHours(2).AddMinutes(60)
        );

        _ongoingTrainingsQuery
            .GetCompletedByUserIdAndDateRangeAsync(
                _userId,
                Arg.Any<DateOnly>(),
                Arg.Any<DateOnly>(),
                Arg.Any<CancellationToken>()
            )
            .Returns(new[] { completed1, completed2 });

        var query = new GetWorkoutDurationHistoryQuery(
            StartDate: startDate,
            EndDate: endDate,
            OverviewType.Daily,
            AggregationType.Average
        );

        var result = await _handler.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        // Same day: 30*60 + 60*60 = 5400 seconds total, average = 2700
        result.Value[0].Value.Should().Be(2700);
    }
}
