using TrackYourLife.Modules.Trainings.Application.Features.WorkoutPlans.Commands.UpdateWorkoutPlan;
using TrackYourLife.Modules.Trainings.Application.UnitTests.Utils;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.Modules.Trainings.Domain.Features.WorkoutPlans;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.WorkoutPlans.Commands.UpdateWorkoutPlan;

public class UpdateWorkoutPlanCommandHandlerTests
{
    private readonly IWorkoutPlansRepository _workoutPlansRepository;
    private readonly ITrainingsQuery _trainingsQuery;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly UpdateWorkoutPlanCommandHandler _handler;
    private readonly UserId _userId;
    private readonly WorkoutPlanId _planId;

    public UpdateWorkoutPlanCommandHandlerTests()
    {
        _workoutPlansRepository = Substitute.For<IWorkoutPlansRepository>();
        _trainingsQuery = Substitute.For<ITrainingsQuery>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _handler = new UpdateWorkoutPlanCommandHandler(
            _workoutPlansRepository,
            _trainingsQuery,
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

        var result = await _handler.Handle(
            new UpdateWorkoutPlanCommand(_planId, "Name", false, []),
            default
        );

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(WorkoutPlanErrors.NotFoundById(_planId));
        _workoutPlansRepository.DidNotReceive().Update(Arg.Any<WorkoutPlan>());
    }

    [Fact]
    public async Task Handle_WhenPlanNotOwned_ShouldReturnFailure()
    {
        var training = TrainingFaker.Generate(userId: _userId);
        var plan = WorkoutPlan
            .Create(_planId, UserId.NewId(), "Plan", false, [training.Id], DateTime.UtcNow)
            .Value;

        _workoutPlansRepository
            .GetByIdAsync(_planId, Arg.Any<CancellationToken>())
            .Returns(plan);
        _trainingsQuery
            .GetByIdAsync(training.Id, Arg.Any<CancellationToken>())
            .Returns(TrainingReadModelFaker.Generate(id: training.Id, userId: _userId));

        var result = await _handler.Handle(
            new UpdateWorkoutPlanCommand(_planId, "Name", false, [training.Id]),
            default
        );

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(WorkoutPlanErrors.NotOwned(_planId));
    }

    [Fact]
    public async Task Handle_WhenTrainingNotFound_ShouldReturnFailure()
    {
        var training = TrainingFaker.Generate(userId: _userId);
        var plan = WorkoutPlan
            .Create(_planId, _userId, "Plan", false, [training.Id], DateTime.UtcNow)
            .Value;

        var missingId = TrainingId.NewId();
        _workoutPlansRepository
            .GetByIdAsync(_planId, Arg.Any<CancellationToken>())
            .Returns(plan);
        _trainingsQuery.GetByIdAsync(missingId, Arg.Any<CancellationToken>()).Returns((TrainingReadModel?)null);

        var result = await _handler.Handle(
            new UpdateWorkoutPlanCommand(_planId, "Name", false, [missingId]),
            default
        );

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TrainingsErrors.NotFoundById(missingId));
    }

    [Fact]
    public async Task Handle_WhenValid_ShouldUpdatePlan()
    {
        var training = TrainingFaker.Generate(userId: _userId);
        var plan = WorkoutPlan
            .Create(_planId, _userId, "Old", false, [training.Id], DateTime.UtcNow)
            .Value;

        _workoutPlansRepository
            .GetByIdAsync(_planId, Arg.Any<CancellationToken>())
            .Returns(plan);
        _workoutPlansRepository.GetActiveByUserIdAsync(_userId, Arg.Any<CancellationToken>()).Returns((WorkoutPlan?)null);
        _trainingsQuery
            .GetByIdAsync(training.Id, Arg.Any<CancellationToken>())
            .Returns(TrainingReadModelFaker.Generate(id: training.Id, userId: _userId));

        var result = await _handler.Handle(
            new UpdateWorkoutPlanCommand(_planId, "New name", false, [training.Id]),
            default
        );

        result.IsSuccess.Should().BeTrue();
        plan.Name.Should().Be("New name");
        _workoutPlansRepository.Received(1).Update(plan);
    }
}
