using TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Commands.UpdateOngoingTraining;
using TrackYourLife.Modules.Trainings.Application.UnitTests.Utils;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.OngoingTrainings.Commands.UpdateOngoingTraining;

public class UpdateOngoingTrainingCommandHandlerTests
{
    private readonly IOngoingTrainingsRepository _ongoingTrainingsRepository;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly UpdateOngoingTrainingCommandHandler _handler;

    private readonly UserId _userId;
    private readonly OngoingTrainingId _ongoingTrainingId;

    public UpdateOngoingTrainingCommandHandlerTests()
    {
        _ongoingTrainingsRepository = Substitute.For<IOngoingTrainingsRepository>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _handler = new UpdateOngoingTrainingCommandHandler(
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
        _ongoingTrainingsRepository
            .GetByIdAsync(_ongoingTrainingId, Arg.Any<CancellationToken>())
            .Returns((OngoingTraining?)null);

        var result = await _handler.Handle(
            new UpdateOngoingTrainingCommand(_ongoingTrainingId, 300, 45),
            default
        );

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OngoingTrainingErrors.NotFoundById(_ongoingTrainingId));
        _ongoingTrainingsRepository.DidNotReceive().Update(Arg.Any<OngoingTraining>());
    }

    [Fact]
    public async Task Handle_WhenOngoingTrainingBelongsToDifferentUser_ShouldReturnFailure()
    {
        var ongoingTraining = OngoingTrainingFaker.Generate(
            id: _ongoingTrainingId,
            userId: UserId.NewId()
        );
        ongoingTraining.Finish(DateTime.UtcNow, null);

        _ongoingTrainingsRepository
            .GetByIdAsync(_ongoingTrainingId, Arg.Any<CancellationToken>())
            .Returns(ongoingTraining);

        var result = await _handler.Handle(
            new UpdateOngoingTrainingCommand(_ongoingTrainingId, 300, 45),
            default
        );

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OngoingTrainingErrors.NotOwned(_ongoingTrainingId));
        _ongoingTrainingsRepository.DidNotReceive().Update(Arg.Any<OngoingTraining>());
    }

    [Fact]
    public async Task Handle_WhenOngoingTrainingNotFinished_ShouldReturnFailure()
    {
        var ongoingTraining = OngoingTrainingFaker.Generate(
            id: _ongoingTrainingId,
            userId: _userId,
            finishedOnUtc: null
        );

        _ongoingTrainingsRepository
            .GetByIdAsync(_ongoingTrainingId, Arg.Any<CancellationToken>())
            .Returns(ongoingTraining);

        var result = await _handler.Handle(
            new UpdateOngoingTrainingCommand(_ongoingTrainingId, 300, 45),
            default
        );

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OngoingTrainingErrors.NotFinished(_ongoingTrainingId));
        _ongoingTrainingsRepository.DidNotReceive().Update(Arg.Any<OngoingTraining>());
    }

    [Fact]
    public async Task Handle_WhenFinished_ShouldUpdateCompletionMetadataAndPersist()
    {
        var startedOnUtc = DateTime.UtcNow.AddHours(-1);
        var ongoingTraining = OngoingTrainingFaker.Generate(
            id: _ongoingTrainingId,
            userId: _userId,
            startedOnUtc: startedOnUtc,
            finishedOnUtc: DateTime.UtcNow,
            caloriesBurned: 100
        );

        _ongoingTrainingsRepository
            .GetByIdAsync(_ongoingTrainingId, Arg.Any<CancellationToken>())
            .Returns(ongoingTraining);

        const int durationMinutes = 40;
        const int caloriesBurned = 450;
        var result = await _handler.Handle(
            new UpdateOngoingTrainingCommand(_ongoingTrainingId, caloriesBurned, durationMinutes),
            default
        );

        result.IsSuccess.Should().BeTrue();
        ongoingTraining.FinishedOnUtc.Should().Be(startedOnUtc.AddMinutes(durationMinutes));
        ongoingTraining.CaloriesBurned.Should().Be(caloriesBurned);
        _ongoingTrainingsRepository.Received(1).Update(ongoingTraining);
    }
}
