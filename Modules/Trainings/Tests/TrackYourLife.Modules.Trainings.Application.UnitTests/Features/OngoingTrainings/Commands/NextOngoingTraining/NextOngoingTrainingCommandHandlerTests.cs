using TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Commands.NextOngoingTraining;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.OngoingTrainings.Commands.NextOngoingTraining;

public class NextOngoingTrainingCommandHandlerTests
{
    private readonly IOngoingTrainingsRepository _ongoingTrainingsRepository;
    private readonly IExercisesHistoriesQuery _exercisesHistoriesQuery;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly NextOngoingTrainingCommandHandler _handler;

    private readonly UserId _userId;
    private readonly OngoingTrainingId _ongoingTrainingId;

    public NextOngoingTrainingCommandHandlerTests()
    {
        _ongoingTrainingsRepository = Substitute.For<IOngoingTrainingsRepository>();
        _exercisesHistoriesQuery = Substitute.For<IExercisesHistoriesQuery>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _handler = new NextOngoingTrainingCommandHandler(
            _ongoingTrainingsRepository,
            _exercisesHistoriesQuery,
            _userIdentifierProvider
        );

        _userId = UserId.NewId();
        _ongoingTrainingId = OngoingTrainingId.NewId();

        _userIdentifierProvider.UserId.Returns(_userId);
        _exercisesHistoriesQuery
            .GetByOngoingTrainingIdAsync(_ongoingTrainingId, Arg.Any<CancellationToken>())
            .Returns(Enumerable.Empty<ExerciseHistoryReadModel>());
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
        await _exercisesHistoriesQuery.DidNotReceive().GetByOngoingTrainingIdAsync(
            Arg.Any<OngoingTrainingId>(),
            Arg.Any<CancellationToken>()
        );
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
        await _exercisesHistoriesQuery.DidNotReceive().GetByOngoingTrainingIdAsync(
            Arg.Any<OngoingTrainingId>(),
            Arg.Any<CancellationToken>()
        );
    }

    [Fact]
    public async Task Handle_WhenOngoingTrainingIsFinished_ShouldReturnFailure()
    {
        // Arrange
        var ongoingTraining = OngoingTrainingFaker.Generate(
            id: _ongoingTrainingId,
            userId: _userId
        );

        // Manually finish the training to simulate already finished state
        ongoingTraining.Finish(DateTime.UtcNow, null);

        _ongoingTrainingsRepository
            .GetByIdAsync(_ongoingTrainingId, Arg.Any<CancellationToken>())
            .Returns(ongoingTraining);

        var command = new NextOngoingTrainingCommand(_ongoingTrainingId);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OngoingTrainingErrors.AlreadyFinished(_ongoingTrainingId));
        _ongoingTrainingsRepository.DidNotReceive().Update(Arg.Any<OngoingTraining>());
        await _exercisesHistoriesQuery.DidNotReceive().GetByOngoingTrainingIdAsync(
            Arg.Any<OngoingTrainingId>(),
            Arg.Any<CancellationToken>()
        );
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
