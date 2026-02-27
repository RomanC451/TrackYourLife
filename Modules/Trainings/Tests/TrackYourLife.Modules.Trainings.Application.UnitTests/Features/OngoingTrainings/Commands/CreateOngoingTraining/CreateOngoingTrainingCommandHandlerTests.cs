using TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Commands.CreateOngoingTraining;
using TrackYourLife.Modules.Trainings.Application.UnitTests.Utils;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.OngoingTrainings.Commands.CreateOngoingTraining;

public class CreateOngoingTrainingCommandHandlerTests
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly ITrainingsRepository _trainingsRepository;
    private readonly IOngoingTrainingsRepository _ongoingTrainingsRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly CreateOngoingTrainingCommandHandler _handler;

    private readonly UserId _userId;
    private readonly TrainingId _trainingId;

    public CreateOngoingTrainingCommandHandlerTests()
    {
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _trainingsRepository = Substitute.For<ITrainingsRepository>();
        _ongoingTrainingsRepository = Substitute.For<IOngoingTrainingsRepository>();
        _dateTimeProvider = Substitute.For<IDateTimeProvider>();
        _handler = new CreateOngoingTrainingCommandHandler(
            _userIdentifierProvider,
            _trainingsRepository,
            _ongoingTrainingsRepository,
            _dateTimeProvider
        );

        _userId = UserId.NewId();
        _trainingId = TrainingId.NewId();

        _userIdentifierProvider.UserId.Returns(_userId);
        _dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);
    }

    [Fact]
    public async Task Handle_WhenTrainingNotFound_ShouldReturnFailure()
    {
        // Arrange
        _trainingsRepository
            .GetByIdAsync(_trainingId, Arg.Any<CancellationToken>())
            .Returns((Training?)null);

        var command = new CreateOngoingTrainingCommand(_trainingId);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TrainingsErrors.NotFoundById(_trainingId));
        await _ongoingTrainingsRepository
            .DidNotReceive()
            .AddAsync(Arg.Any<OngoingTraining>(), Arg.Any<CancellationToken>());
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

        var command = new CreateOngoingTrainingCommand(_trainingId);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TrainingsErrors.NotOwned(_trainingId));
        await _ongoingTrainingsRepository
            .DidNotReceive()
            .AddAsync(Arg.Any<OngoingTraining>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenAnotherTrainingAlreadyStarted_ShouldReturnFailure()
    {
        // Arrange
        var training = TrainingFaker.Generate(id: _trainingId, userId: _userId);
        var differentTrainingId = TrainingId.NewId();
        var alreadyStartedTraining = OngoingTrainingFaker.Generate(
            training: TrainingFaker.Generate(id: differentTrainingId, userId: _userId)
        );

        _trainingsRepository
            .GetByIdAsync(_trainingId, Arg.Any<CancellationToken>())
            .Returns(training);
        _ongoingTrainingsRepository
            .GetUnfinishedByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(alreadyStartedTraining);

        var command = new CreateOngoingTrainingCommand(_trainingId);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result
            .Error.Should()
            .Be(OngoingTrainingErrors.AnotherTrainingAlreadyStarted(alreadyStartedTraining.Id));
        await _ongoingTrainingsRepository
            .DidNotReceive()
            .AddAsync(Arg.Any<OngoingTraining>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenSameTrainingAlreadyStarted_ShouldReturnFailure()
    {
        // Arrange
        var training = TrainingFaker.Generate(id: _trainingId, userId: _userId);
        var alreadyStartedTraining = OngoingTrainingFaker.Generate(training: training);

        _trainingsRepository
            .GetByIdAsync(_trainingId, Arg.Any<CancellationToken>())
            .Returns(training);
        _ongoingTrainingsRepository
            .GetUnfinishedByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(alreadyStartedTraining);

        var command = new CreateOngoingTrainingCommand(_trainingId);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OngoingTrainingErrors.AlreadyStarted(alreadyStartedTraining.Id));
        await _ongoingTrainingsRepository
            .DidNotReceive()
            .AddAsync(Arg.Any<OngoingTraining>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenAllValidationsPass_ShouldCreateOngoingTraining()
    {
        // Arrange
        var training = TrainingFaker.Generate(id: _trainingId, userId: _userId);

        _trainingsRepository
            .GetByIdAsync(_trainingId, Arg.Any<CancellationToken>())
            .Returns(training);
        _ongoingTrainingsRepository
            .GetUnfinishedByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns((OngoingTraining?)null);

        var command = new CreateOngoingTrainingCommand(_trainingId);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _ongoingTrainingsRepository
            .Received(1)
            .AddAsync(
                Arg.Is<OngoingTraining>(ot =>
                    ot.UserId == _userId && ot.Training.Id == _trainingId
                ),
                Arg.Any<CancellationToken>()
            );
    }
}

