using TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Commands.DeleteOngoingTraining;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.OngoingTrainings.Commands.DeleteOngoingTraining;

public class DeleteOngoingTrainingCommandHandlerTests
{
    private readonly IOngoingTrainingsRepository _ongoingTrainingRepository;
    private readonly DeleteOngoingTrainingCommandHandler _handler;

    private readonly TrainingId _trainingId;

    public DeleteOngoingTrainingCommandHandlerTests()
    {
        _ongoingTrainingRepository = Substitute.For<IOngoingTrainingsRepository>();
        _handler = new DeleteOngoingTrainingCommandHandler(_ongoingTrainingRepository);

        _trainingId = TrainingId.NewId();
    }

    [Fact]
    public async Task Handle_WhenOngoingTrainingNotFound_ShouldReturnFailure()
    {
        // Arrange
        _ongoingTrainingRepository
            .GetByTrainingIdAndNotFinishedAsync(_trainingId, Arg.Any<CancellationToken>())
            .Returns((OngoingTraining?)null);

        var command = new DeleteOngoingTrainingCommand(_trainingId);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OngoingTrainingErrors.NotFound);
        _ongoingTrainingRepository.DidNotReceive().Remove(Arg.Any<OngoingTraining>());
    }

    [Fact]
    public async Task Handle_WhenOngoingTrainingFound_ShouldDeleteOngoingTraining()
    {
        // Arrange
        var ongoingTraining = OngoingTrainingFaker.Generate();

        _ongoingTrainingRepository
            .GetByTrainingIdAndNotFinishedAsync(_trainingId, Arg.Any<CancellationToken>())
            .Returns(ongoingTraining);

        var command = new DeleteOngoingTrainingCommand(_trainingId);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _ongoingTrainingRepository.Received(1).Remove(ongoingTraining);
    }
}

