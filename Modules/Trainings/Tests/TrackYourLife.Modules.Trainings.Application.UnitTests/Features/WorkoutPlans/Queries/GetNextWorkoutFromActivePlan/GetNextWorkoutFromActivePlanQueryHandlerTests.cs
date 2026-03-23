using TrackYourLife.Modules.Trainings.Application.Features.WorkoutPlans.Queries.GetNextWorkoutFromActivePlan;
using TrackYourLife.Modules.Trainings.Application.UnitTests.Utils;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.Modules.Trainings.Domain.Features.WorkoutPlans;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.WorkoutPlans.Queries.GetNextWorkoutFromActivePlan;

public class GetNextWorkoutFromActivePlanQueryHandlerTests
{
    private readonly IWorkoutPlansQuery _workoutPlansQuery;
    private readonly IOngoingTrainingsQuery _ongoingTrainingsQuery;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly GetNextWorkoutFromActivePlanQueryHandler _handler;
    private readonly UserId _userId;

    public GetNextWorkoutFromActivePlanQueryHandlerTests()
    {
        _workoutPlansQuery = Substitute.For<IWorkoutPlansQuery>();
        _ongoingTrainingsQuery = Substitute.For<IOngoingTrainingsQuery>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _handler = new GetNextWorkoutFromActivePlanQueryHandler(
            _workoutPlansQuery,
            _ongoingTrainingsQuery,
            _userIdentifierProvider
        );

        _userId = UserId.NewId();
        _userIdentifierProvider.UserId.Returns(_userId);
    }

    [Fact]
    public async Task Handle_WhenNoLastCompletedTraining_ShouldReturnFirstWorkout()
    {
        var firstTraining = TrainingReadModelFaker.Generate();
        var secondTraining = TrainingReadModelFaker.Generate();
        var activePlan = WorkoutPlanReadModelFaker.Generate(
            userId: _userId,
            isActive: true,
            trainings: [firstTraining, secondTraining]
        );

        _workoutPlansQuery
            .GetActiveByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(activePlan);
        _ongoingTrainingsQuery
            .GetLastCompletedByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns((OngoingTrainingReadModel?)null);

        var result = await _handler.Handle(new GetNextWorkoutFromActivePlanQuery(), default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(firstTraining.Id);
    }

    [Fact]
    public async Task Handle_WhenLastCompletedIsLastInPlan_ShouldWrapToFirstWorkout()
    {
        var firstTraining = TrainingReadModelFaker.Generate();
        var secondTraining = TrainingReadModelFaker.Generate();
        var activePlan = WorkoutPlanReadModelFaker.Generate(
            userId: _userId,
            isActive: true,
            trainings: [firstTraining, secondTraining]
        );

        var lastCompleted = OngoingTrainingReadModelFaker.Generate(
            userId: _userId,
            finishedOnUtc: DateTime.UtcNow,
            training: secondTraining
        );

        _workoutPlansQuery
            .GetActiveByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(activePlan);
        _ongoingTrainingsQuery
            .GetLastCompletedByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(lastCompleted);

        var result = await _handler.Handle(new GetNextWorkoutFromActivePlanQuery(), default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(firstTraining.Id);
    }

    [Fact]
    public async Task Handle_WhenLastCompletedTrainingNotInPlan_ShouldReturnFirstWorkout()
    {
        var firstTraining = TrainingReadModelFaker.Generate();
        var secondTraining = TrainingReadModelFaker.Generate();
        var activePlan = WorkoutPlanReadModelFaker.Generate(
            userId: _userId,
            isActive: true,
            trainings: [firstTraining, secondTraining]
        );

        var notInPlanTraining = TrainingReadModelFaker.Generate();
        var lastCompleted = OngoingTrainingReadModelFaker.Generate(
            userId: _userId,
            finishedOnUtc: DateTime.UtcNow,
            training: notInPlanTraining
        );

        _workoutPlansQuery
            .GetActiveByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(activePlan);
        _ongoingTrainingsQuery
            .GetLastCompletedByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(lastCompleted);

        var result = await _handler.Handle(new GetNextWorkoutFromActivePlanQuery(), default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(firstTraining.Id);
    }
}
