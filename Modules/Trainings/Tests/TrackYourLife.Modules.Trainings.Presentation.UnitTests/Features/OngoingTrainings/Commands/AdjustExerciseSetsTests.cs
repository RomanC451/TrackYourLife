using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Commands.AdjustExerciseSets;
using TrackYourLife.Modules.Trainings.Domain.Core;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.Modules.Trainings.Presentation.Features.OngoingTrainings.Commands;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Presentation.UnitTests;

namespace TrackYourLife.Modules.Trainings.Presentation.UnitTests.Features.OngoingTrainings.Commands;

public class AdjustExerciseSetsTests
{
    private readonly ISender _sender;
    private readonly AdjustExerciseSets _endpoint;

    public AdjustExerciseSetsTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new AdjustExerciseSets(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandSucceeds_ShouldReturnNoContent()
    {
        // Arrange
        var ongoingTrainingId = OngoingTrainingId.NewId();
        var exerciseId = ExerciseId.NewId();
        var newExerciseSets = new List<ExerciseSet>
        {
            ExerciseSet.Create(Guid.NewGuid(), "Set 1", 0, 12, "reps", 60, "kg").Value,
            ExerciseSet.Create(Guid.NewGuid(), "Set 2", 1, 12, "reps", 60, "kg").Value,
        };

        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues = new RouteValueDictionary
        {
            { "id", ongoingTrainingId.Value },
        };
        _endpoint.SetHttpContext(httpContext);

        _sender
            .Send(Arg.Any<AdjustExerciseSetsCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        var request = new AdjustExerciseSetsRequest(
            ExerciseId: exerciseId,
            NewExerciseSets: newExerciseSets
        );

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<NoContent>();

        await _sender
            .Received(1)
            .Send(
                Arg.Is<AdjustExerciseSetsCommand>(c =>
                    c.OngoingTrainingId == ongoingTrainingId
                    && c.ExerciseId == exerciseId
                    && c.NewExerciseSets.Count == 2
                ),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandFails_ShouldReturnProblemDetails()
    {
        // Arrange
        var ongoingTrainingId = OngoingTrainingId.NewId();
        var exerciseId = ExerciseId.NewId();
        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues = new RouteValueDictionary
        {
            { "id", ongoingTrainingId.Value },
        };
        _endpoint.SetHttpContext(httpContext);

        var error = new Error("NotFound", "Ongoing training not found");
        _sender
            .Send(Arg.Any<AdjustExerciseSetsCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure(error));

        var request = new AdjustExerciseSetsRequest(
            ExerciseId: exerciseId,
            NewExerciseSets: new List<ExerciseSet>()
        );

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
