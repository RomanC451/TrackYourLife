using Microsoft.AspNetCore.Http.HttpResults;
using TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Commands.NextOngoingTraining;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.Modules.Trainings.Presentation.Features.OngoingTrainings.Commands;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Presentation.UnitTests.Features.OngoingTrainings.Commands;

public class NextOngoingTrainingTests
{
    private readonly ISender _sender;
    private readonly NextOngoingTraining _endpoint;

    public NextOngoingTrainingTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new NextOngoingTraining(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandSucceeds_ShouldReturnNoContent()
    {
        // Arrange
        var ongoingTrainingId = OngoingTrainingId.NewId();

        _sender
            .Send(Arg.Any<NextOngoingTrainingCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        var request = new NextOngoingTrainingRequest(OngoingTrainingId: ongoingTrainingId);

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<NoContent>();

        await _sender
            .Received(1)
            .Send(
                Arg.Is<NextOngoingTrainingCommand>(c => c.OngoingTrainingId == ongoingTrainingId),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandFails_ShouldReturnProblemDetails()
    {
        // Arrange
        var ongoingTrainingId = OngoingTrainingId.NewId();
        var error = new Error("NotFound", "Ongoing training not found");
        _sender
            .Send(Arg.Any<NextOngoingTrainingCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure(error));

        var request = new NextOngoingTrainingRequest(OngoingTrainingId: ongoingTrainingId);

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
