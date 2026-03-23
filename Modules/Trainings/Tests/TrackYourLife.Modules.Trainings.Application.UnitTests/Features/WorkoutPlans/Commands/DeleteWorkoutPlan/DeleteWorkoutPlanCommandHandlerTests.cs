using TrackYourLife.Modules.Trainings.Application.Features.WorkoutPlans.Commands.DeleteWorkoutPlan;
using TrackYourLife.Modules.Trainings.Application.UnitTests.Utils;
using TrackYourLife.Modules.Trainings.Domain.Features.WorkoutPlans;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.WorkoutPlans.Commands.DeleteWorkoutPlan;

public class DeleteWorkoutPlanCommandHandlerTests
{
    private readonly IWorkoutPlansRepository _workoutPlansRepository;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly DeleteWorkoutPlanCommandHandler _handler;
    private readonly UserId _userId;
    private readonly WorkoutPlanId _planId;

    public DeleteWorkoutPlanCommandHandlerTests()
    {
        _workoutPlansRepository = Substitute.For<IWorkoutPlansRepository>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _handler = new DeleteWorkoutPlanCommandHandler(
            _workoutPlansRepository,
            _userIdentifierProvider
        );

        _userId = UserId.NewId();
        _planId = WorkoutPlanId.NewId();
        _userIdentifierProvider.UserId.Returns(_userId);
    }

    [Fact]
    public async Task Handle_WhenPlanNotFound_ShouldReturnFailure()
    {
        _workoutPlansRepository
            .GetByIdAsync(_planId, Arg.Any<CancellationToken>())
            .Returns((WorkoutPlan?)null);

        var result = await _handler.Handle(new DeleteWorkoutPlanCommand(_planId), default);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(WorkoutPlanErrors.NotFoundById(_planId));
        _workoutPlansRepository.DidNotReceive().Remove(Arg.Any<WorkoutPlan>());
    }

    [Fact]
    public async Task Handle_WhenPlanNotOwned_ShouldReturnFailure()
    {
        var plan = WorkoutPlan
            .Create(
                _planId,
                UserId.NewId(),
                "Plan",
                false,
                [TrainingFaker.Generate().Id],
                DateTime.UtcNow
            )
            .Value;

        _workoutPlansRepository
            .GetByIdAsync(_planId, Arg.Any<CancellationToken>())
            .Returns(plan);

        var result = await _handler.Handle(new DeleteWorkoutPlanCommand(_planId), default);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(WorkoutPlanErrors.NotOwned(_planId));
        _workoutPlansRepository.DidNotReceive().Remove(Arg.Any<WorkoutPlan>());
    }

    [Fact]
    public async Task Handle_WhenValid_ShouldRemovePlan()
    {
        var training = TrainingFaker.Generate(userId: _userId);
        var plan = WorkoutPlan
            .Create(_planId, _userId, "Plan", true, [training.Id], DateTime.UtcNow)
            .Value;

        _workoutPlansRepository
            .GetByIdAsync(_planId, Arg.Any<CancellationToken>())
            .Returns(plan);

        var result = await _handler.Handle(new DeleteWorkoutPlanCommand(_planId), default);

        result.IsSuccess.Should().BeTrue();
        _workoutPlansRepository.Received(1).Remove(plan);
    }
}
