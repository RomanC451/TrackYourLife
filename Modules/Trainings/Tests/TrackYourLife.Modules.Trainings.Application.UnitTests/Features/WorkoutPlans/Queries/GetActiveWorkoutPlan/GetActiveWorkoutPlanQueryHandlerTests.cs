using TrackYourLife.Modules.Trainings.Application.Features.WorkoutPlans.Queries.GetActiveWorkoutPlan;
using TrackYourLife.Modules.Trainings.Application.UnitTests.Utils;
using TrackYourLife.Modules.Trainings.Domain.Features.WorkoutPlans;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.WorkoutPlans.Queries.GetActiveWorkoutPlan;

public class GetActiveWorkoutPlanQueryHandlerTests
{
    private readonly IWorkoutPlansQuery _workoutPlansQuery;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly GetActiveWorkoutPlanQueryHandler _handler;
    private readonly UserId _userId;

    public GetActiveWorkoutPlanQueryHandlerTests()
    {
        _workoutPlansQuery = Substitute.For<IWorkoutPlansQuery>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _handler = new GetActiveWorkoutPlanQueryHandler(
            _workoutPlansQuery,
            _userIdentifierProvider
        );

        _userId = UserId.NewId();
        _userIdentifierProvider.UserId.Returns(_userId);
    }

    [Fact]
    public async Task Handle_WhenNoActivePlan_ShouldReturnFailure()
    {
        _workoutPlansQuery
            .GetActiveByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns((WorkoutPlanReadModel?)null);

        var result = await _handler.Handle(new GetActiveWorkoutPlanQuery(), default);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(WorkoutPlanErrors.ActivePlanNotFound);
    }

    [Fact]
    public async Task Handle_WhenActivePlanExists_ShouldReturnPlan()
    {
        var plan = WorkoutPlanReadModelFaker.Generate(userId: _userId, isActive: true);
        _workoutPlansQuery
            .GetActiveByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(plan);

        var result = await _handler.Handle(new GetActiveWorkoutPlanQuery(), default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(plan);
    }
}
