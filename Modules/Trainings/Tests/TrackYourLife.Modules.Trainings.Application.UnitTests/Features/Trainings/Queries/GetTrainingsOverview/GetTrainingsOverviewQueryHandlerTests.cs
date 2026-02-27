using TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetTrainingsOverview;
using TrackYourLife.Modules.Trainings.Application.UnitTests.Utils;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.Trainings.Queries.GetTrainingsOverview;

public class GetTrainingsOverviewQueryHandlerTests
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IOngoingTrainingsQuery _ongoingTrainingsQuery;
    private readonly GetTrainingsOverviewQueryHandler _handler;

    private readonly UserId _userId;

    public GetTrainingsOverviewQueryHandlerTests()
    {
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _ongoingTrainingsQuery = Substitute.For<IOngoingTrainingsQuery>();
        _handler = new GetTrainingsOverviewQueryHandler(
            _userIdentifierProvider,
            _ongoingTrainingsQuery
        );

        _userId = UserId.NewId();
        _userIdentifierProvider.UserId.Returns(_userId);
    }

    [Fact]
    public async Task Handle_WhenNoCompletedWorkouts_ShouldReturnZeroOverview()
    {
        // Arrange
        _ongoingTrainingsQuery
            .GetCompletedByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(Array.Empty<OngoingTrainingReadModel>());
        _ongoingTrainingsQuery
            .GetUnfinishedByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns((OngoingTrainingReadModel?)null);

        var query = new GetTrainingsOverviewQuery(StartDate: null, EndDate: null);

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.TotalWorkoutsCompleted.Should().Be(0);
        result.Value.TotalTrainingTimeSeconds.Should().Be(0);
        result.Value.TotalCaloriesBurned.Should().BeNull();
        result.Value.HasActiveTraining.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WhenCompletedWorkoutsExist_ShouldReturnAggregatedOverview()
    {
        // Arrange
        var started1 = DateTime.UtcNow.AddHours(-2);
        var finished1 = DateTime.UtcNow.AddHours(-1);
        var started2 = DateTime.UtcNow.AddHours(-3);
        var finished2 = DateTime.UtcNow.AddHours(-2);

        var completed1 = OngoingTrainingReadModelFaker.Generate(
            userId: _userId,
            startedOnUtc: started1,
            finishedOnUtc: finished1,
            caloriesBurned: 200
        );
        completed1.Training = TrainingReadModelFaker.Generate();

        var completed2 = OngoingTrainingReadModelFaker.Generate(
            userId: _userId,
            startedOnUtc: started2,
            finishedOnUtc: finished2,
            caloriesBurned: 300
        );
        completed2.Training = TrainingReadModelFaker.Generate();

        _ongoingTrainingsQuery
            .GetCompletedByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(new[] { completed1, completed2 });
        _ongoingTrainingsQuery
            .GetUnfinishedByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns((OngoingTrainingReadModel?)null);

        var query = new GetTrainingsOverviewQuery(StartDate: null, EndDate: null);

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.TotalWorkoutsCompleted.Should().Be(2);
        result.Value.TotalTrainingTimeSeconds.Should().Be(7200); // 2h + 1h in seconds
        result.Value.TotalCaloriesBurned.Should().Be(500);
        result.Value.HasActiveTraining.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WhenActiveTrainingExists_ShouldSetHasActiveTrainingTrue()
    {
        // Arrange
        var activeTraining = OngoingTrainingReadModelFaker.Generate(
            userId: _userId,
            finishedOnUtc: null
        );
        activeTraining.Training = TrainingReadModelFaker.Generate();

        _ongoingTrainingsQuery
            .GetCompletedByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(Array.Empty<OngoingTrainingReadModel>());
        _ongoingTrainingsQuery
            .GetUnfinishedByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(activeTraining);

        var query = new GetTrainingsOverviewQuery(StartDate: null, EndDate: null);

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.HasActiveTraining.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenStartDateProvided_ShouldFilterCompletedWorkouts()
    {
        // Arrange
        var startDate = DateTime.UtcNow.AddDays(-7);
        var beforeStart = OngoingTrainingReadModelFaker.Generate(
            userId: _userId,
            startedOnUtc: startDate.AddDays(-2),
            finishedOnUtc: startDate.AddDays(-1)
        );
        beforeStart.Training = TrainingReadModelFaker.Generate();
        var afterStart = OngoingTrainingReadModelFaker.Generate(
            userId: _userId,
            startedOnUtc: startDate.AddDays(1),
            finishedOnUtc: startDate.AddDays(2)
        );
        afterStart.Training = TrainingReadModelFaker.Generate();

        _ongoingTrainingsQuery
            .GetCompletedByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(new[] { beforeStart, afterStart });
        _ongoingTrainingsQuery
            .GetUnfinishedByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns((OngoingTrainingReadModel?)null);

        var query = new GetTrainingsOverviewQuery(
            StartDate: DateOnly.FromDateTime(startDate),
            EndDate: null
        );

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.TotalWorkoutsCompleted.Should().Be(1);
    }

    [Fact]
    public async Task Handle_ShouldUseCurrentUserId()
    {
        // Arrange
        _ongoingTrainingsQuery
            .GetCompletedByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(Array.Empty<OngoingTrainingReadModel>());
        _ongoingTrainingsQuery
            .GetUnfinishedByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns((OngoingTrainingReadModel?)null);

        var query = new GetTrainingsOverviewQuery(StartDate: null, EndDate: null);

        // Act
        await _handler.Handle(query, default);

        // Assert
        await _ongoingTrainingsQuery
            .Received(1)
            .GetCompletedByUserIdAsync(_userId, Arg.Any<CancellationToken>());
        await _ongoingTrainingsQuery
            .Received(1)
            .GetUnfinishedByUserIdAsync(_userId, Arg.Any<CancellationToken>());
    }
}
