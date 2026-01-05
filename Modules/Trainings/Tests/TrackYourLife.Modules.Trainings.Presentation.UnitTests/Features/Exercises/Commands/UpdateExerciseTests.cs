using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using TrackYourLife.Modules.Trainings.Application.Features.Exercises.Commands.UpdateExercise;
using TrackYourLife.Modules.Trainings.Domain.Core;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Presentation.Features.Exercises.Commands;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Presentation.UnitTests;

namespace TrackYourLife.Modules.Trainings.Presentation.UnitTests.Features.Exercises.Commands;

public class UpdateExerciseTests
{
    private readonly ISender _sender;
    private readonly UpdateExercise _endpoint;

    public UpdateExerciseTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new UpdateExercise(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandSucceeds_ShouldReturnNoContent()
    {
        // Arrange
        var exerciseId = ExerciseId.NewId();
        var exerciseSets = new List<ExerciseSet>
        {
            ExerciseSet.Create(Guid.NewGuid(), "Set 1", 0, 12, "reps", 60, "kg").Value,
        };

        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues = new RouteValueDictionary
        {
            { "id", exerciseId.Value },
        };
        _endpoint.SetHttpContext(httpContext);

        _sender
            .Send(Arg.Any<UpdateExerciseCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        var request = new UpdateExerciseRequest(
            Name: "Updated Exercise",
            MuscleGroups: new List<string> { "Back", "Biceps" },
            Difficulty: Difficulty.Hard,
            Description: "Updated description",
            VideoUrl: "https://example.com/updated-video.mp4",
            PictureUrl: "https://example.com/updated-picture.jpg",
            Equipment: "Dumbbell",
            ExerciseSets: exerciseSets
        );

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<NoContent>();

        await _sender
            .Received(1)
            .Send(
                Arg.Is<UpdateExerciseCommand>(c =>
                    c.Id == exerciseId
                    && c.Name == "Updated Exercise"
                    && c.MuscleGroups.Count == 2
                    && c.Difficulty == Difficulty.Hard
                ),
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
            .Send(Arg.Any<UpdateExerciseCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure(error));

        var request = new UpdateExerciseRequest(
            Name: "Updated Exercise",
            MuscleGroups: new List<string> { "Chest" },
            Difficulty: Difficulty.Medium,
            Description: null,
            VideoUrl: null,
            PictureUrl: null,
            Equipment: null,
            ExerciseSets: new List<ExerciseSet>()
        );

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
