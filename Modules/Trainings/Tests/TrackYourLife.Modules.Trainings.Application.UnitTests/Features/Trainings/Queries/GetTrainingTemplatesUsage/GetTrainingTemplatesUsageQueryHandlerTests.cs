using TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetTrainingTemplatesUsage;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.Trainings.Queries.GetTrainingTemplatesUsage;

public class GetTrainingTemplatesUsageQueryHandlerTests
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IOngoingTrainingsQuery _ongoingTrainingsQuery;
    private readonly IExercisesHistoriesQuery _exercisesHistoriesQuery;
    private readonly GetTrainingTemplatesUsageQueryHandler _handler;

    private readonly UserId _userId;

    public GetTrainingTemplatesUsageQueryHandlerTests()
    {
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _ongoingTrainingsQuery = Substitute.For<IOngoingTrainingsQuery>();
        _exercisesHistoriesQuery = Substitute.For<IExercisesHistoriesQuery>();
        _handler = new GetTrainingTemplatesUsageQueryHandler(
            _userIdentifierProvider,
            _ongoingTrainingsQuery,
            _exercisesHistoriesQuery
        );

        _userId = UserId.NewId();
        _userIdentifierProvider.UserId.Returns(_userId);
    }

    [Fact]
    public async Task Handle_WhenNoCompletedWorkouts_ShouldReturnEmptyList()
    {
        // Arrange
        _ongoingTrainingsQuery
            .GetCompletedByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(Array.Empty<OngoingTrainingReadModel>());
        _exercisesHistoriesQuery
            .GetByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(Array.Empty<ExerciseHistoryReadModel>());

        var query = new GetTrainingTemplatesUsageQuery();

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WhenCompletedWorkoutsExist_ShouldReturnTemplateUsage()
    {
        // Arrange
        var trainingId = TrainingId.NewId();
        var trainingName = "Test Training";
        var training = TrainingReadModelFaker.Generate(id: trainingId, name: trainingName);
        var ongoingId1 = OngoingTrainingId.NewId();
        var ongoingId2 = OngoingTrainingId.NewId();

        var completed1 = OngoingTrainingReadModelFaker.Generate(
            id: ongoingId1,
            userId: _userId,
            startedOnUtc: DateTime.UtcNow.AddHours(-2),
            finishedOnUtc: DateTime.UtcNow.AddHours(-1),
            caloriesBurned: 250
        );
        completed1.Training = training;

        var completed2 = OngoingTrainingReadModelFaker.Generate(
            id: ongoingId2,
            userId: _userId,
            startedOnUtc: DateTime.UtcNow.AddHours(-4),
            finishedOnUtc: DateTime.UtcNow.AddHours(-3),
            caloriesBurned: 300
        );
        completed2.Training = training;

        _ongoingTrainingsQuery
            .GetCompletedByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(new[] { completed1, completed2 });
        _exercisesHistoriesQuery
            .GetByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(Array.Empty<ExerciseHistoryReadModel>()); // No skipped

        var query = new GetTrainingTemplatesUsageQuery();

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var usage = result.Value.ToList();
        usage.Should().HaveCount(1);
        usage[0].TrainingId.Should().Be(trainingId);
        usage[0].TrainingName.Should().Be(trainingName);
        usage[0].TotalCompleted.Should().Be(2);
        usage[0].TotalFullyCompleted.Should().Be(2);
        usage[0].TotalWithSkippedExercises.Should().Be(0);
        usage[0].CompletionRate.Should().Be(100);
    }

    [Fact]
    public async Task Handle_WhenDateRangeProvided_ShouldCallGetCompletedByUserIdAndDateRangeAsync()
    {
        // Arrange
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
        _exercisesHistoriesQuery
            .GetByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(Array.Empty<ExerciseHistoryReadModel>());

        var query = new GetTrainingTemplatesUsageQuery(StartDate: startDate, EndDate: endDate);

        // Act
        await _handler.Handle(query, default);

        // Assert
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
        // Arrange
        _ongoingTrainingsQuery
            .GetCompletedByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(Array.Empty<OngoingTrainingReadModel>());
        _exercisesHistoriesQuery
            .GetByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(Array.Empty<ExerciseHistoryReadModel>());

        var query = new GetTrainingTemplatesUsageQuery();

        // Act
        await _handler.Handle(query, default);

        // Assert
        await _ongoingTrainingsQuery
            .Received(1)
            .GetCompletedByUserIdAsync(_userId, Arg.Any<CancellationToken>());
        await _exercisesHistoriesQuery
            .Received(1)
            .GetByUserIdAsync(_userId, Arg.Any<CancellationToken>());
    }
}
