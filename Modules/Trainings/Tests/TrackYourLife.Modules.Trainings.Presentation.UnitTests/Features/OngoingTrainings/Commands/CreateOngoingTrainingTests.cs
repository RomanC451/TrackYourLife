using Microsoft.AspNetCore.Http.HttpResults;
using TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Commands.CreateOngoingTraining;
using TrackYourLife.Modules.Trainings.Application.UnitTests.Utils;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.Modules.Trainings.Presentation.Features.OngoingTrainings.Commands;
using TrackYourLife.SharedLib.Contracts.Shared;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Presentation.UnitTests.Features.OngoingTrainings.Commands;

public class CreateOngoingTrainingTests
{
    private readonly ISender _sender;
    private readonly CreateOngoingTraining _endpoint;

    public CreateOngoingTrainingTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new CreateOngoingTraining(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandSucceeds_ShouldReturnCreatedWithIdResponse()
    {
        // Arrange
        var trainingId = TrainingId.NewId();
        var ongoingTrainingId = OngoingTrainingId.NewId();

        _sender
            .Send(Arg.Any<CreateOngoingTrainingCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(ongoingTrainingId));

        var request = new CreateOngoingTrainingRequest(TrainingId: trainingId);

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var createdResult = result.Should().BeOfType<Created<IdResponse>>().Subject;
        createdResult.Value.Should().NotBeNull();
        createdResult.Value!.Id.Should().Be(ongoingTrainingId);

        await _sender
            .Received(1)
            .Send(
                Arg.Is<CreateOngoingTrainingCommand>(c => c.TrainingId == trainingId),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandFails_ShouldReturnProblemDetails()
    {
        // Arrange
        var trainingId = TrainingId.NewId();
        var error = new Error("NotFound", "Training not found");
        _sender
            .Send(Arg.Any<CreateOngoingTrainingCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<OngoingTrainingId>(error));

        var request = new CreateOngoingTrainingRequest(TrainingId: trainingId);

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
