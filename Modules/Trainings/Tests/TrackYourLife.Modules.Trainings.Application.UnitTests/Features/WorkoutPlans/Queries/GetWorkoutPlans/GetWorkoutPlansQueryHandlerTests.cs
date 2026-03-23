using TrackYourLife.Modules.Trainings.Application.Features.WorkoutPlans.Queries.GetWorkoutPlans;
using TrackYourLife.Modules.Trainings.Application.UnitTests.Utils;
using TrackYourLife.Modules.Trainings.Domain.Features.WorkoutPlans;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.WorkoutPlans.Queries.GetWorkoutPlans;

public class GetWorkoutPlansQueryHandlerTests
{
    private readonly IWorkoutPlansQuery _workoutPlansQuery;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly GetWorkoutPlansQueryHandler _handler;
    private readonly UserId _userId;

    public GetWorkoutPlansQueryHandlerTests()
    {
        _workoutPlansQuery = Substitute.For<IWorkoutPlansQuery>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _handler = new GetWorkoutPlansQueryHandler(_workoutPlansQuery, _userIdentifierProvider);

        _userId = UserId.NewId();
        _userIdentifierProvider.UserId.Returns(_userId);
    }

    [Fact]
    public async Task Handle_ShouldReturnPlansForCurrentUser()
    {
        var plans = new[]
        {
            WorkoutPlanReadModelFaker.Generate(userId: _userId),
            WorkoutPlanReadModelFaker.Generate(userId: _userId),
        };
        _workoutPlansQuery
            .GetByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(plans);

        var result = await _handler.Handle(new GetWorkoutPlansQuery(), default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(plans);
    }
}
