using TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Commands.FinishOngoingTraining;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.OngoingTrainings.Commands.FinishOngoingTraining;

public class FinishOngoingTrainingCommandHandlerTests
{
    private readonly IOngoingTrainingsRepository _ongoingTrainingsRepository;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly FinishOngoingTrainingCommandHandler _handler;

    private readonly UserId _userId;
    private readonly OngoingTrainingId _ongoingTrainingId;

    public FinishOngoingTrainingCommandHandlerTests()
    {
        _ongoingTrainingsRepository = Substitute.For<IOngoingTrainingsRepository>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _dateTimeProvider = Substitute.For<IDateTimeProvider>();
        _handler = new FinishOngoingTrainingCommandHandler(
            _ongoingTrainingsRepository,
            _userIdentifierProvider,
            _dateTimeProvider
        );

        _userId = UserId.NewId();
        _ongoingTrainingId = OngoingTrainingId.NewId();

        _userIdentifierProvider.UserId.Returns(_userId);
        _dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);
    }

    [Fact]
    public async Task Handle_WhenOngoingTrainingNotFound_ShouldReturnFailure()
    {
        // Arrange
        _ongoingTrainingsRepository
            .GetByIdAsync(_ongoingTrainingId, Arg.Any<CancellationToken>())
            .Returns((OngoingTraining?)null);

        var command = new FinishOngoingTrainingCommand(_ongoingTrainingId);

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

        var command = new FinishOngoingTrainingCommand(_ongoingTrainingId);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OngoingTrainingErrors.NotOwned(_ongoingTrainingId));
        _ongoingTrainingsRepository.DidNotReceive().Update(Arg.Any<OngoingTraining>());
    }

    [Fact]
    public async Task Handle_WhenOngoingTrainingAlreadyFinished_ShouldReturnFailure()
    {
        // Arrange
        var ongoingTraining = OngoingTrainingFaker.Generate(
            id: _ongoingTrainingId,
            userId: _userId
        );

        // Manually finish the training to simulate already finished state
        ongoingTraining.Finish(DateTime.UtcNow);

        _ongoingTrainingsRepository
            .GetByIdAsync(_ongoingTrainingId, Arg.Any<CancellationToken>())
            .Returns(ongoingTraining);

        var command = new FinishOngoingTrainingCommand(_ongoingTrainingId);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OngoingTrainingErrors.AlreadyFinished(_ongoingTrainingId));
        _ongoingTrainingsRepository.DidNotReceive().Update(Arg.Any<OngoingTraining>());
    }

    [Fact]
    public async Task Handle_WhenAllValidationsPass_ShouldFinishOngoingTraining()
    {
        // Arrange
        var ongoingTraining = OngoingTrainingFaker.Generate(
            id: _ongoingTrainingId,
            userId: _userId
        );

        _ongoingTrainingsRepository
            .GetByIdAsync(_ongoingTrainingId, Arg.Any<CancellationToken>())
            .Returns(ongoingTraining);

        var command = new FinishOngoingTrainingCommand(_ongoingTrainingId);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _ongoingTrainingsRepository.Received(1).Update(ongoingTraining);
    }
}
