using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Commands.FinishOngoingTraining;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.Modules.Trainings.Presentation.Features.OngoingTrainings.Commands;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Presentation.UnitTests;

namespace TrackYourLife.Modules.Trainings.Presentation.UnitTests.Features.OngoingTrainings.Commands;

public class FinishOngoingTrainingTests
{
    private readonly ISender _sender;
    private readonly FinishOngoingTraining _endpoint;

    public FinishOngoingTrainingTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new FinishOngoingTraining(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandSucceeds_ShouldReturnNoContent()
    {
        // Arrange
        var ongoingTrainingId = OngoingTrainingId.NewId();
        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues = new RouteValueDictionary
        {
            { "id", ongoingTrainingId.Value },
        };
        _endpoint.SetHttpContext(httpContext);

        _sender
            .Send(Arg.Any<FinishOngoingTrainingCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        var request = new FinishOngoingTrainingRequest();

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<NoContent>();

        await _sender
            .Received(1)
            .Send(
                Arg.Is<FinishOngoingTrainingCommand>(c => c.Id == ongoingTrainingId && c.CaloriesBurned == null),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandSucceeds_WithCaloriesBurned_ShouldReturnNoContent()
    {
        // Arrange
        var ongoingTrainingId = OngoingTrainingId.NewId();
        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues = new RouteValueDictionary
        {
            { "id", ongoingTrainingId.Value },
        };
        _endpoint.SetHttpContext(httpContext);

        _sender
            .Send(Arg.Any<FinishOngoingTrainingCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        var caloriesBurned = 500;
        var request = new FinishOngoingTrainingRequest(caloriesBurned);

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<NoContent>();

        await _sender
            .Received(1)
            .Send(
                Arg.Is<FinishOngoingTrainingCommand>(c => c.Id == ongoingTrainingId && c.CaloriesBurned == caloriesBurned),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandFails_ShouldReturnProblemDetails()
    {
        // Arrange
        var ongoingTrainingId = OngoingTrainingId.NewId();
        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues = new RouteValueDictionary
        {
            { "id", ongoingTrainingId.Value },
        };
        _endpoint.SetHttpContext(httpContext);

        var error = new Error("NotFound", "Ongoing training not found");
        _sender
            .Send(Arg.Any<FinishOngoingTrainingCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure(error));

        var request = new FinishOngoingTrainingRequest();

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
