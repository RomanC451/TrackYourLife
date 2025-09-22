using TrackYourLife.Modules.Trainings.Application.Features.Trainings.Commands.DeleteTraining;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.Trainings.Commands.DeleteTraining;

public class DeleteTrainingCommandHandlerTests
{
    private readonly ITrainingsRepository _trainingsRepository;
    private readonly IOngoingTrainingsQuery _ongoingTrainingsQuery;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly DeleteTrainingCommandHandler _handler;

    private readonly UserId _userId;
    private readonly TrainingId _trainingId;

    public DeleteTrainingCommandHandlerTests()
    {
        _trainingsRepository = Substitute.For<ITrainingsRepository>();
        _ongoingTrainingsQuery = Substitute.For<IOngoingTrainingsQuery>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _handler = new DeleteTrainingCommandHandler(
            _trainingsRepository,
            _ongoingTrainingsQuery,
            _userIdentifierProvider
        );

        _userId = UserId.NewId();
        _trainingId = TrainingId.NewId();

        _userIdentifierProvider.UserId.Returns(_userId);
    }

    [Fact]
    public async Task Handle_WhenTrainingNotFound_ShouldReturnFailure()
    {
        // Arrange
        _trainingsRepository
            .GetByIdAsync(_trainingId, Arg.Any<CancellationToken>())
            .Returns((Training?)null);

        var command = new DeleteTrainingCommand(_trainingId, false);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TrainingsErrors.NotFoundById(_trainingId));
        _trainingsRepository.DidNotReceive().Remove(Arg.Any<Training>());
    }

    [Fact]
    public async Task Handle_WhenTrainingBelongsToDifferentUser_ShouldReturnFailure()
    {
        // Arrange
        var differentUserId = UserId.NewId();
        var training = TrainingFaker.Generate(id: _trainingId, userId: differentUserId);

        _trainingsRepository
            .GetByIdAsync(_trainingId, Arg.Any<CancellationToken>())
            .Returns(training);

        var command = new DeleteTrainingCommand(_trainingId, false);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TrainingsErrors.NotOwned(_trainingId));
        _trainingsRepository.DidNotReceive().Remove(Arg.Any<Training>());
    }

    [Fact]
    public async Task Handle_WhenTrainingIsOngoingAndForceFalse_ShouldReturnFailure()
    {
        // Arrange
        var training = TrainingFaker.Generate(id: _trainingId, userId: _userId);

        _trainingsRepository
            .GetByIdAsync(_trainingId, Arg.Any<CancellationToken>())
            .Returns(training);
        _ongoingTrainingsQuery
            .IsTrainingOngoingAsync(_trainingId, Arg.Any<CancellationToken>())
            .Returns(true);

        var command = new DeleteTrainingCommand(_trainingId, false);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TrainingsErrors.OngoingTraining(_trainingId));
        _trainingsRepository.DidNotReceive().Remove(Arg.Any<Training>());
    }

    [Fact]
    public async Task Handle_WhenTrainingIsOngoingAndForceTrue_ShouldDeleteTraining()
    {
        // Arrange
        var training = TrainingFaker.Generate(id: _trainingId, userId: _userId);

        _trainingsRepository
            .GetByIdAsync(_trainingId, Arg.Any<CancellationToken>())
            .Returns(training);
        _ongoingTrainingsQuery
            .IsTrainingOngoingAsync(_trainingId, Arg.Any<CancellationToken>())
            .Returns(true);

        var command = new DeleteTrainingCommand(_trainingId, true);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _trainingsRepository.Received(1).Remove(training);
    }

    [Fact]
    public async Task Handle_WhenTrainingNotOngoing_ShouldDeleteTraining()
    {
        // Arrange
        var training = TrainingFaker.Generate(id: _trainingId, userId: _userId);

        _trainingsRepository
            .GetByIdAsync(_trainingId, Arg.Any<CancellationToken>())
            .Returns(training);
        _ongoingTrainingsQuery
            .IsTrainingOngoingAsync(_trainingId, Arg.Any<CancellationToken>())
            .Returns(false);

        var command = new DeleteTrainingCommand(_trainingId, false);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _trainingsRepository.Received(1).Remove(training);
    }
}

