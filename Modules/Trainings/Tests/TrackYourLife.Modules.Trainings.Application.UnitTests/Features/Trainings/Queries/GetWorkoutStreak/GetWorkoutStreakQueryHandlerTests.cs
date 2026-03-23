using TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetWorkoutStreak;
using TrackYourLife.Modules.Trainings.Application.UnitTests.Utils;
using TrackYourLife.Modules.Trainings.Contracts.Dtos;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.Trainings.Queries.GetWorkoutStreak;

public class GetWorkoutStreakQueryHandlerTests
{
    private readonly IOngoingTrainingsQuery _ongoingTrainingsQuery;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly GetWorkoutStreakQueryHandler _handler;
    private readonly UserId _userId;

    public GetWorkoutStreakQueryHandlerTests()
    {
        _ongoingTrainingsQuery = Substitute.For<IOngoingTrainingsQuery>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _handler = new GetWorkoutStreakQueryHandler(
            _userIdentifierProvider,
            _ongoingTrainingsQuery
        );

        _userId = UserId.NewId();
        _userIdentifierProvider.UserId.Returns(_userId);
    }

    [Fact]
    public async Task Handle_WhenNoCompletedWorkouts_ShouldReturnZeroStreak()
    {
        _ongoingTrainingsQuery
            .GetCompletedByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(Array.Empty<OngoingTrainingReadModel>());

        var result = await _handler.Handle(new GetWorkoutStreakQuery(), default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(new WorkoutStreakDto(0));
    }

    [Fact]
    public async Task Handle_WhenLastWorkoutOlderThanYesterday_ShouldReturnZeroStreak()
    {
        var old = OngoingTrainingReadModelFaker.Generate(
            userId: _userId,
            finishedOnUtc: DateTime.UtcNow.AddDays(-3)
        );

        _ongoingTrainingsQuery
            .GetCompletedByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns([old]);

        var result = await _handler.Handle(new GetWorkoutStreakQuery(), default);

        result.IsSuccess.Should().BeTrue();
        result.Value.DayStreak.Should().Be(0);
    }

    [Fact]
    public async Task Handle_WhenWorkoutTodayOnly_ShouldReturnStreakOfOne()
    {
        var today = OngoingTrainingReadModelFaker.Generate(
            userId: _userId,
            finishedOnUtc: DateTime.UtcNow
        );

        _ongoingTrainingsQuery
            .GetCompletedByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns([today]);

        var result = await _handler.Handle(new GetWorkoutStreakQuery(), default);

        result.IsSuccess.Should().BeTrue();
        result.Value.DayStreak.Should().Be(1);
    }

    [Fact]
    public async Task Handle_WhenWorkoutsOnConsecutiveDaysIncludingToday_ShouldCountStreak()
    {
        var today = OngoingTrainingReadModelFaker.Generate(
            userId: _userId,
            finishedOnUtc: DateTime.UtcNow
        );
        var yesterday = OngoingTrainingReadModelFaker.Generate(
            userId: _userId,
            finishedOnUtc: DateTime.UtcNow.AddDays(-1)
        );

        _ongoingTrainingsQuery
            .GetCompletedByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns([today, yesterday]);

        var result = await _handler.Handle(new GetWorkoutStreakQuery(), default);

        result.IsSuccess.Should().BeTrue();
        result.Value.DayStreak.Should().Be(2);
    }

    [Fact]
    public async Task Handle_WhenWorkoutYesterdayOnly_ShouldReturnStreakOfOne()
    {
        var yesterday = OngoingTrainingReadModelFaker.Generate(
            userId: _userId,
            finishedOnUtc: DateTime.UtcNow.AddDays(-1)
        );

        _ongoingTrainingsQuery
            .GetCompletedByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns([yesterday]);

        var result = await _handler.Handle(new GetWorkoutStreakQuery(), default);

        result.IsSuccess.Should().BeTrue();
        result.Value.DayStreak.Should().Be(1);
    }
}
