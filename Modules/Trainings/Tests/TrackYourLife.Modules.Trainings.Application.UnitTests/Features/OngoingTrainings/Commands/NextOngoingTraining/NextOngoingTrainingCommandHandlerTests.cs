using TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Commands.NextOngoingTraining;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.OngoingTrainings.Commands.NextOngoingTraining;

public class NextOngoingTrainingCommandHandlerTests
{
    private readonly IOngoingTrainingsRepository _ongoingTrainingsRepository;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly NextOngoingTrainingCommandHandler _handler;

    private readonly UserId _userId;
    private readonly OngoingTrainingId _ongoingTrainingId;

    public NextOngoingTrainingCommandHandlerTests()
    {
        _ongoingTrainingsRepository = Substitute.For<IOngoingTrainingsRepository>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _handler = new NextOngoingTrainingCommandHandler(
            _ongoingTrainingsRepository,
            _userIdentifierProvider
        );

        _userId = UserId.NewId();
        _ongoingTrainingId = OngoingTrainingId.NewId();

        _userIdentifierProvider.UserId.Returns(_userId);
    }

    [Fact]
    public async Task Handle_WhenOngoingTrainingNotFound_ShouldReturnFailure()
    {
        // Arrange
        _ongoingTrainingsRepository
            .GetByIdAsync(_ongoingTrainingId, Arg.Any<CancellationToken>())
            .Returns((OngoingTraining?)null);

        var command = new NextOngoingTrainingCommand(_ongoingTrainingId);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OngoingTrainingErrors.NotFoundById(_ongoingTrainingId));
        _ongoingTrainingsRepository.DidNotReceive().Update(Arg.Any<OngoingTraining>());
    }

    [Fact]
    public async Task Handle_WhenOngoingTrainingBelongsToDifferentUser_ShouldReturnFailure()
    {
        // Arrange
        var differentUserId = UserId.NewId();
        var ongoingTraining = OngoingTrainingFaker.Generate(
            id: _ongoingTrainingId,
            userId: differentUserId
        );

        _ongoingTrainingsRepository
            .GetByIdAsync(_ongoingTrainingId, Arg.Any<CancellationToken>())
            .Returns(ongoingTraining);

        var command = new NextOngoingTrainingCommand(_ongoingTrainingId);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OngoingTrainingErrors.NotOwned(_ongoingTrainingId));
        _ongoingTrainingsRepository.DidNotReceive().Update(Arg.Any<OngoingTraining>());
    }

    [Fact]
    public async Task Handle_WhenNextOperationFails_ShouldReturnFailure()
    {
        // Arrange
        // Create a training with 2 exercises, each with exactly 2 sets
        var exerciseSets1 = new List<ExerciseSet>
        {
            new WeightBasedExerciseSet(Guid.NewGuid(), "Set 1", 0, 10, 50),
            new WeightBasedExerciseSet(Guid.NewGuid(), "Set 2", 1, 10, 50),
        };
        var exerciseSets2 = new List<ExerciseSet>
        {
            new WeightBasedExerciseSet(Guid.NewGuid(), "Set 1", 0, 10, 50),
            new WeightBasedExerciseSet(Guid.NewGuid(), "Set 2", 1, 10, 50),
        };

        var exercise1 = ExerciseFaker.Generate(exerciseSets: exerciseSets1);
        var exercise2 = ExerciseFaker.Generate(exerciseSets: exerciseSets2);
        var training = TrainingFaker.Generate(
            exercises: new List<Exercise> { exercise1, exercise2 }
        );

        // Create ongoing training at the last set of the last exercise (exerciseIndex=1, setIndex=1)
        var ongoingTraining = OngoingTrainingFaker.Generate(
            id: _ongoingTrainingId,
            userId: _userId,
            exerciseIndex: 1, // Last exercise
            setIndex: 1, // Last set
            training: training
        );

        _ongoingTrainingsRepository
            .GetByIdAsync(_ongoingTrainingId, Arg.Any<CancellationToken>())
            .Returns(ongoingTraining);

        var command = new NextOngoingTrainingCommand(_ongoingTrainingId);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        _ongoingTrainingsRepository.DidNotReceive().Update(Arg.Any<OngoingTraining>());
    }

    [Fact]
    public async Task Handle_WhenAllValidationsPass_ShouldUpdateOngoingTraining()
    {
        // Arrange
        var ongoingTraining = OngoingTrainingFaker.Generate(
            id: _ongoingTrainingId,
            userId: _userId,
            0
        );

        _ongoingTrainingsRepository
            .GetByIdAsync(_ongoingTrainingId, Arg.Any<CancellationToken>())
            .Returns(ongoingTraining);

        var command = new NextOngoingTrainingCommand(_ongoingTrainingId);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _ongoingTrainingsRepository.Received(1).Update(ongoingTraining);
    }
}
