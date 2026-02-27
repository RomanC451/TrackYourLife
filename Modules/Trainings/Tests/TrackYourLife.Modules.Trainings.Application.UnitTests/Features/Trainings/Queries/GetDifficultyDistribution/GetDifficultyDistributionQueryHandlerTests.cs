using TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetDifficultyDistribution;
using TrackYourLife.Modules.Trainings.Application.UnitTests.Utils;
using TrackYourLife.Modules.Trainings.Domain.Core;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.Trainings.Queries.GetDifficultyDistribution;

public class GetDifficultyDistributionQueryHandlerTests
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IOngoingTrainingsQuery _ongoingTrainingsQuery;
    private readonly GetDifficultyDistributionQueryHandler _handler;

    private readonly UserId _userId;

    public GetDifficultyDistributionQueryHandlerTests()
    {
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _ongoingTrainingsQuery = Substitute.For<IOngoingTrainingsQuery>();
        _handler = new GetDifficultyDistributionQueryHandler(
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

        var query = new GetDifficultyDistributionQuery(StartDate: null, EndDate: null);

        var result = await _handler.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WhenCompletedWorkoutsExist_ShouldReturnDifficultyDistribution()
    {
        var trainingEasy = TrainingReadModelFaker.Generate(difficulty: Difficulty.Easy);
        var trainingMedium = TrainingReadModelFaker.Generate(difficulty: Difficulty.Medium);
        var completed1 = OngoingTrainingReadModelFaker.Generate(
            userId: _userId,
            finishedOnUtc: DateTime.UtcNow.AddHours(-1)
        );
        completed1.Training = trainingEasy;
        var completed2 = OngoingTrainingReadModelFaker.Generate(
            userId: _userId,
            finishedOnUtc: DateTime.UtcNow.AddHours(-2)
        );
        completed2.Training = trainingEasy;
        var completed3 = OngoingTrainingReadModelFaker.Generate(
            userId: _userId,
            finishedOnUtc: DateTime.UtcNow.AddHours(-3)
        );
        completed3.Training = trainingMedium;

        _ongoingTrainingsQuery
            .GetCompletedByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(new[] { completed1, completed2, completed3 });

        var query = new GetDifficultyDistributionQuery(StartDate: null, EndDate: null);

        var result = await _handler.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result
            .Value.Should()
            .Contain(d => d.Difficulty == nameof(Difficulty.Easy) && d.WorkoutCount == 2);
        result
            .Value.Should()
            .Contain(d => d.Difficulty == nameof(Difficulty.Medium) && d.WorkoutCount == 1);
    }

    [Fact]
    public async Task Handle_ShouldUseCurrentUserId()
    {
        _ongoingTrainingsQuery
            .GetCompletedByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(Array.Empty<OngoingTrainingReadModel>());

        var query = new GetDifficultyDistributionQuery(StartDate: null, EndDate: null);

        await _handler.Handle(query, default);

        await _ongoingTrainingsQuery
            .Received(1)
            .GetCompletedByUserIdAsync(_userId, Arg.Any<CancellationToken>());
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

        var query = new GetDifficultyDistributionQuery(StartDate: startDate, EndDate: endDate);

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
    public async Task Handle_WhenWorkoutHasNullTraining_ShouldUseEasyAsDefaultDifficulty()
    {
        var completed = OngoingTrainingReadModelFaker.Generate(
            userId: _userId,
            finishedOnUtc: DateTime.UtcNow.AddHours(-1)
        );
        completed.Training = null!;

        _ongoingTrainingsQuery
            .GetCompletedByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(new[] { completed });

        var query = new GetDifficultyDistributionQuery(StartDate: null, EndDate: null);

        var result = await _handler.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value[0].Difficulty.Should().Be(nameof(Difficulty.Easy));
        result.Value[0].WorkoutCount.Should().Be(1);
    }
}
