using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using TrackYourLife.Modules.Trainings.Application.Features.Exercises.Commands.DeleteExercise;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Presentation.Features.Exercises.Commands;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Presentation.UnitTests;

namespace TrackYourLife.Modules.Trainings.Presentation.UnitTests.Features.Exercises.Commands;

public class DeleteExerciseTests
{
    private readonly ISender _sender;
    private readonly DeleteExercise _endpoint;

    public DeleteExerciseTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new DeleteExercise(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandSucceeds_ShouldReturnNoContent()
    {
        // Arrange
        var exerciseId = ExerciseId.NewId();
        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues = new RouteValueDictionary
        {
            { "id", exerciseId.Value },
        };
        _endpoint.SetHttpContext(httpContext);

        _sender
            .Send(Arg.Any<DeleteExerciseCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        var request = new DeleteExerciseRequest(ForceDelete: false);

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<NoContent>();

        await _sender
            .Received(1)
            .Send(
                Arg.Is<DeleteExerciseCommand>(c => c.ExerciseId == exerciseId && !c.ForceDelete),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenForceDeleteIsTrue_ShouldPassForceDeleteFlag()
    {
        // Arrange
        var exerciseId = ExerciseId.NewId();
        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues = new RouteValueDictionary
        {
            { "id", exerciseId.Value },
        };
        _endpoint.SetHttpContext(httpContext);

        _sender
            .Send(Arg.Any<DeleteExerciseCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        var request = new DeleteExerciseRequest(ForceDelete: true);

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<NoContent>();

        await _sender
            .Received(1)
            .Send(
                Arg.Is<DeleteExerciseCommand>(c => c.ExerciseId == exerciseId && c.ForceDelete),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandFails_ShouldReturnProblemDetails()
    {
        // Arrange
        var exerciseId = ExerciseId.NewId();
        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues = new RouteValueDictionary
        {
            { "id", exerciseId.Value },
        };
        _endpoint.SetHttpContext(httpContext);

        var error = new Error("NotFound", "Exercise not found");
        _sender
            .Send(Arg.Any<DeleteExerciseCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure(error));

        var request = new DeleteExerciseRequest(ForceDelete: false);

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
