using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using TrackYourLife.Modules.Trainings.Application.Features.Trainings.Commands.UpdateTraining;
using TrackYourLife.Modules.Trainings.Domain.Core;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.Modules.Trainings.Presentation.Features.Trainings.Commands;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Presentation.UnitTests;

namespace TrackYourLife.Modules.Trainings.Presentation.UnitTests.Features.Trainings.Commands;

public class UpdateTrainingTests
{
    private readonly ISender _sender;
    private readonly UpdateTraining _endpoint;

    public UpdateTrainingTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new UpdateTraining(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandSucceeds_ShouldReturnNoContent()
    {
        // Arrange
        var trainingId = TrainingId.NewId();
        var exerciseId1 = ExerciseId.NewId();
        var exerciseId2 = ExerciseId.NewId();

        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues = new RouteValueDictionary
        {
            { "id", trainingId.Value },
        };
        _endpoint.SetHttpContext(httpContext);

        _sender
            .Send(Arg.Any<UpdateTrainingCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        var request = new UpdateTrainingRequest(
            Name: "Updated Training",
            MuscleGroups: new List<string> { "Back", "Biceps" },
            Difficulty: Difficulty.Hard,
            Duration: 90,
            RestSeconds: 120,
            Description: "Updated description",
            ExercisesIds: new List<ExerciseId> { exerciseId1, exerciseId2 }
        );

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<NoContent>();

        await _sender
            .Received(1)
            .Send(
                Arg.Is<UpdateTrainingCommand>(c =>
                    c.TrainingId == trainingId
                    && c.Name == "Updated Training"
                    && c.MuscleGroups.Count == 2
                    && c.Difficulty == Difficulty.Hard
                    && c.Duration == 90
                    && c.RestSeconds == 120
                ),
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

        var error = new Error("NotFound", "Training not found");
        _sender
            .Send(Arg.Any<UpdateTrainingCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure(error));

        var request = new UpdateTrainingRequest(
            Name: "Updated Training",
            MuscleGroups: new List<string> { "Chest" },
            Difficulty: Difficulty.Medium,
            Duration: 60,
            RestSeconds: 90,
            Description: null,
            ExercisesIds: new List<ExerciseId>()
        );

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
