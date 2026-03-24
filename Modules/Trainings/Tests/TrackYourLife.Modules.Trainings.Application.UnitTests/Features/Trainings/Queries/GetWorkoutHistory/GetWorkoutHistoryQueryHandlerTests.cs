using TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetWorkoutHistory;
using TrackYourLife.Modules.Trainings.Application.UnitTests.Utils;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.Modules.Trainings.Domain.Features.TrainingExercises;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.Trainings.Queries.GetWorkoutHistory;

public class GetWorkoutHistoryQueryHandlerTests
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IOngoingTrainingsQuery _ongoingTrainingsQuery;
    private readonly IExercisesHistoriesQuery _exercisesHistoriesQuery;
    private readonly GetWorkoutHistoryQueryHandler _handler;

    private readonly UserId _userId;

    public GetWorkoutHistoryQueryHandlerTests()
    {
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _ongoingTrainingsQuery = Substitute.For<IOngoingTrainingsQuery>();
        _exercisesHistoriesQuery = Substitute.For<IExercisesHistoriesQuery>();
        _handler = new GetWorkoutHistoryQueryHandler(
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
        _ongoingTrainingsQuery
            .GetCompletedByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(Array.Empty<OngoingTrainingReadModel>());

        var query = new GetWorkoutHistoryQuery(StartDate: null, EndDate: null);

        var result = await _handler.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
        await _exercisesHistoriesQuery
            .DidNotReceive()
            .GetCompletedByUserIdAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenCompletedWorkoutsExist_ShouldReturnWorkoutHistory()
    {
        var training = TrainingReadModelFaker.Generate(name: "Full Body");
        var exerciseId1 = ExerciseId.NewId();
        var exerciseId2 = ExerciseId.NewId();
        var exerciseId3 = ExerciseId.NewId();
        training = training with
        {
            TrainingExercises =
            [
                new TrainingExerciseReadModel(training.Id, exerciseId1, 0),
                new TrainingExerciseReadModel(training.Id, exerciseId2, 1),
                new TrainingExerciseReadModel(training.Id, exerciseId3, 2),
            ],
        };

        var started = DateTime.UtcNow.AddHours(-2);
        var finished = started.AddMinutes(45);
        var completed = OngoingTrainingReadModelFaker.Generate(
            userId: _userId,
            startedOnUtc: started,
            finishedOnUtc: finished,
            training: training
        );

        _ongoingTrainingsQuery
            .GetCompletedByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(new[] { completed });

        _exercisesHistoriesQuery
            .GetCompletedByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(
                new[]
                {
                    new ExerciseHistoryReadModel(
                        ExerciseHistoryId.NewId(),
                        completed.Id,
                        exerciseId1,
                        Status: ExerciseStatus.Completed
                    ),
                    new ExerciseHistoryReadModel(
                        ExerciseHistoryId.NewId(),
                        completed.Id,
                        exerciseId2,
                        Status: ExerciseStatus.Completed
                    ),
                    new ExerciseHistoryReadModel(
                        ExerciseHistoryId.NewId(),
                        completed.Id,
                        exerciseId3,
                        Status: ExerciseStatus.Skipped
                    ),
                }
            );

        var query = new GetWorkoutHistoryQuery(StartDate: null, EndDate: null);

        var result = await _handler.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        var dto = result.Value.First();
        dto.Id.Should().Be(completed.Id);
        dto.TrainingId.Should().Be(training.Id);
        dto.TrainingName.Should().Be("Full Body");
        dto.StartedOnUtc.Should().Be(started);
        dto.FinishedOnUtc.Should().Be(finished);
        dto.DurationSeconds.Should().Be(45 * 60);
        dto.TotalExercisesCount.Should().Be(3);
        dto.CompletedExercisesCount.Should().Be(2);
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

        var query = new GetWorkoutHistoryQuery(StartDate: startDate, EndDate: endDate);

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
    public async Task Handle_WhenWorkoutHasNullTraining_ShouldExcludeFromResult()
    {
        var completed = OngoingTrainingReadModelFaker.Generate(
            userId: _userId,
            finishedOnUtc: DateTime.UtcNow.AddHours(-1)
        );
        completed.Training = null!;

        _ongoingTrainingsQuery
            .GetCompletedByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(new[] { completed });

        var query = new GetWorkoutHistoryQuery(StartDate: null, EndDate: null);

        var result = await _handler.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldUseCurrentUserId()
    {
        _ongoingTrainingsQuery
            .GetCompletedByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(Array.Empty<OngoingTrainingReadModel>());

        var query = new GetWorkoutHistoryQuery(StartDate: null, EndDate: null);

        await _handler.Handle(query, default);

        await _ongoingTrainingsQuery
            .Received(1)
            .GetCompletedByUserIdAsync(_userId, Arg.Any<CancellationToken>());
    }
}
