using TrackYourLife.Modules.Trainings.Application.Features.WorkoutPlans.Commands.CreateWorkoutPlan;
using TrackYourLife.Modules.Trainings.Application.UnitTests.Utils;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.Modules.Trainings.Domain.Features.WorkoutPlans;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.WorkoutPlans.Commands.CreateWorkoutPlan;

public class CreateWorkoutPlanCommandHandlerTests
{
    private readonly IWorkoutPlansRepository _workoutPlansRepository;
    private readonly IWorkoutPlansQuery _workoutPlansQuery;
    private readonly ITrainingsQuery _trainingsQuery;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly CreateWorkoutPlanCommandHandler _handler;
    private readonly UserId _userId;

    public CreateWorkoutPlanCommandHandlerTests()
    {
        _workoutPlansRepository = Substitute.For<IWorkoutPlansRepository>();
        _workoutPlansQuery = Substitute.For<IWorkoutPlansQuery>();
        _trainingsQuery = Substitute.For<ITrainingsQuery>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _dateTimeProvider = Substitute.For<IDateTimeProvider>();
        _handler = new CreateWorkoutPlanCommandHandler(
            _workoutPlansRepository,
            _workoutPlansQuery,
            _trainingsQuery,
            _userIdentifierProvider,
            _dateTimeProvider
        );

        _userId = UserId.NewId();
        _userIdentifierProvider.UserId.Returns(_userId);
        _dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);
    }

    [Fact]
    public async Task Handle_WhenCreatingActivePlan_ShouldDeactivateCurrentActivePlan()
    {
        var training = TrainingReadModelFaker.Generate(userId: _userId);
        _trainingsQuery.GetByIdAsync(training.Id, Arg.Any<CancellationToken>()).Returns(training);
        _workoutPlansQuery
            .GetByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns([WorkoutPlanReadModelFaker.Generate(userId: _userId)]);

        var oldActivePlan = WorkoutPlan
            .Create(
                WorkoutPlanId.NewId(),
                _userId,
                "Old plan",
                true,
                [training.Id],
                DateTime.UtcNow.AddDays(-1)
            )
            .Value;

        _workoutPlansRepository
            .GetActiveByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(oldActivePlan);

        var command = new CreateWorkoutPlanCommand("New Plan", true, [training.Id]);

        var result = await _handler.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        oldActivePlan.IsActive.Should().BeFalse();
        _workoutPlansRepository.Received(1).Update(oldActivePlan);
        await _workoutPlansRepository
            .Received(1)
            .AddAsync(Arg.Any<WorkoutPlan>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenCreatingFirstPlan_ShouldMakeItActive()
    {
        var training = TrainingReadModelFaker.Generate(userId: _userId);
        _trainingsQuery.GetByIdAsync(training.Id, Arg.Any<CancellationToken>()).Returns(training);
        _workoutPlansQuery.GetByUserIdAsync(_userId, Arg.Any<CancellationToken>()).Returns([]);
        _workoutPlansRepository
            .GetActiveByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns((WorkoutPlan?)null);

        WorkoutPlan? createdPlan = null;
        _workoutPlansRepository
            .When(x => x.AddAsync(Arg.Any<WorkoutPlan>(), Arg.Any<CancellationToken>()))
            .Do(callInfo => createdPlan = callInfo.Arg<WorkoutPlan>());

        var command = new CreateWorkoutPlanCommand("First Plan", false, [training.Id]);

        var result = await _handler.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        createdPlan.Should().NotBeNull();
        createdPlan!.IsActive.Should().BeTrue();
    }
}
