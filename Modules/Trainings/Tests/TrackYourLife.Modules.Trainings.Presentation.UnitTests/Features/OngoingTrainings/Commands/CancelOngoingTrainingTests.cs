using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Commands.DeleteOngoingTraining;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.Modules.Trainings.Presentation.Features.OngoingTrainings.Commands;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Presentation.UnitTests;

namespace TrackYourLife.Modules.Trainings.Presentation.UnitTests.Features.OngoingTrainings.Commands;

public class CancelOngoingTrainingTests
{
    private readonly ISender _sender;
    private readonly DeleteOngoingTraining _endpoint;

    public CancelOngoingTrainingTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new DeleteOngoingTraining(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandSucceeds_ShouldReturnNoContent()
    {
        // Arrange
        var trainingId = TrainingId.NewId();
        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues = new RouteValueDictionary
        {
            { "id", trainingId.Value },
        };
        _endpoint.SetHttpContext(httpContext);

        _sender
            .Send(Arg.Any<DeleteOngoingTrainingCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        var result = await _endpoint.ExecuteAsync(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<NoContent>();

        await _sender
            .Received(1)
            .Send(
                Arg.Is<DeleteOngoingTrainingCommand>(c => c.TrainingId == trainingId),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandFails_ShouldReturnProblemDetails()
    {
        // Arrange
        var trainingId = TrainingId.NewId();
        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues = new RouteValueDictionary
        {
            { "id", trainingId.Value },
        };
        _endpoint.SetHttpContext(httpContext);

        var error = new Error("NotFound", "Ongoing training not found");
        _sender
            .Send(Arg.Any<DeleteOngoingTrainingCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure(error));

        // Act
        var result = await _endpoint.ExecuteAsync(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
