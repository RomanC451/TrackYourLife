using TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetWorkoutFrequency;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.Trainings.Queries.GetWorkoutFrequency;

public class GetWorkoutFrequencyQueryHandlerTests
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IOngoingTrainingsQuery _ongoingTrainingsQuery;
    private readonly GetWorkoutFrequencyQueryHandler _handler;

    private readonly UserId _userId;

    public GetWorkoutFrequencyQueryHandlerTests()
    {
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _ongoingTrainingsQuery = Substitute.For<IOngoingTrainingsQuery>();
        _handler = new GetWorkoutFrequencyQueryHandler(
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

        var query = new GetWorkoutFrequencyQuery(
            StartDate: null,
            EndDate: null,
            OverviewType.Daily
        );

        var result = await _handler.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WhenDateRangeProvided_ShouldCallGetCompletedByUserIdAndDateRangeAsync()
    {
        var startDate = DateTime.UtcNow.AddDays(-7);
        var endDate = DateTime.UtcNow;
        _ongoingTrainingsQuery
            .GetCompletedByUserIdAndDateRangeAsync(
                _userId,
                Arg.Any<DateTime>(),
                Arg.Any<DateTime>(),
                Arg.Any<CancellationToken>()
            )
            .Returns(Array.Empty<OngoingTrainingReadModel>());

        var query = new GetWorkoutFrequencyQuery(
            StartDate: startDate,
            EndDate: endDate,
            OverviewType.Daily
        );

        await _handler.Handle(query, default);

        await _ongoingTrainingsQuery
            .Received(1)
            .GetCompletedByUserIdAndDateRangeAsync(
                _userId,
                DateTime.SpecifyKind(startDate, DateTimeKind.Utc),
                DateTime.SpecifyKind(endDate, DateTimeKind.Utc),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task Handle_ShouldUseCurrentUserId()
    {
        _ongoingTrainingsQuery
            .GetCompletedByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(Array.Empty<OngoingTrainingReadModel>());

        var query = new GetWorkoutFrequencyQuery(
            StartDate: null,
            EndDate: null,
            OverviewType.Daily
        );

        await _handler.Handle(query, default);

        await _ongoingTrainingsQuery
            .Received(1)
            .GetCompletedByUserIdAsync(_userId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenNoDateRangeAndCompletedWorkoutsExist_ShouldReturnFrequencyHistory()
    {
        var finished = DateTime.UtcNow.AddDays(-1);
        var completed = OngoingTrainingReadModelFaker.Generate(
            userId: _userId,
            finishedOnUtc: finished
        );

        _ongoingTrainingsQuery
            .GetCompletedByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(new[] { completed });

        var query = new GetWorkoutFrequencyQuery(
            StartDate: null,
            EndDate: null,
            OverviewType.Daily
        );

        var result = await _handler.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        result.Value[0].WorkoutCount.Should().Be(1);
    }

    [Fact]
    public async Task Handle_WhenOverviewTypeWeekly_ShouldReturnWeeklyFrequency()
    {
        var startDate = DateTime.UtcNow.AddDays(-14);
        var endDate = DateTime.UtcNow;
        var completed = OngoingTrainingReadModelFaker.Generate(
            userId: _userId,
            finishedOnUtc: startDate.AddDays(2)
        );

        _ongoingTrainingsQuery
            .GetCompletedByUserIdAndDateRangeAsync(
                _userId,
                Arg.Any<DateTime>(),
                Arg.Any<DateTime>(),
                Arg.Any<CancellationToken>()
            )
            .Returns(new[] { completed });

        var query = new GetWorkoutFrequencyQuery(
            StartDate: startDate,
            EndDate: endDate,
            OverviewType.Weekly
        );

        var result = await _handler.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Handle_WhenOverviewTypeMonthly_ShouldReturnMonthlyFrequency()
    {
        var startDate = DateTime.UtcNow.AddMonths(-2);
        var endDate = DateTime.UtcNow;
        var completed = OngoingTrainingReadModelFaker.Generate(
            userId: _userId,
            finishedOnUtc: startDate.AddDays(5)
        );

        _ongoingTrainingsQuery
            .GetCompletedByUserIdAndDateRangeAsync(
                _userId,
                Arg.Any<DateTime>(),
                Arg.Any<DateTime>(),
                Arg.Any<CancellationToken>()
            )
            .Returns(new[] { completed });

        var query = new GetWorkoutFrequencyQuery(
            StartDate: startDate,
            EndDate: endDate,
            OverviewType.Monthly
        );

        var result = await _handler.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
    }
}
