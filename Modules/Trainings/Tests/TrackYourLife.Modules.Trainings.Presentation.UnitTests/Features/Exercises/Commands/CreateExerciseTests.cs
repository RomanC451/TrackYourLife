using Microsoft.AspNetCore.Http.HttpResults;
using TrackYourLife.Modules.Trainings.Application.Features.Exercises.Commands.CreateExercise;
using TrackYourLife.Modules.Trainings.Domain.Core;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Presentation.Features.Exercises.Commands;
using TrackYourLife.SharedLib.Contracts.Shared;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Presentation.UnitTests.Features.Exercises.Commands;

public class CreateExerciseTests
{
    private readonly ISender _sender;
    private readonly CreateExercise _endpoint;

    public CreateExerciseTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new CreateExercise(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandSucceeds_ShouldReturnCreatedWithIdResponse()
    {
        // Arrange
        var exerciseId = ExerciseId.NewId();
        var exerciseSets = new List<ExerciseSet>
        {
            ExerciseSet.Create(Guid.NewGuid(), "Set 1", 0, 10, "reps", 50, "kg").Value,
            ExerciseSet.Create(Guid.NewGuid(), "Set 2", 1, 10, "reps", 50, "kg").Value,
        };

        _sender
            .Send(Arg.Any<CreateExerciseCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(exerciseId));

        var request = new CreateExerciseRequest(
            Name: "Test Exercise",
            MuscleGroups: new List<string> { "Chest", "Triceps" },
            Difficulty: Difficulty.Medium,
            PictureUrl: "https://example.com/picture.jpg",
            VideoUrl: "https://example.com/video.mp4",
            Description: "Test description",
            Equipment: "Barbell",
            ExerciseSets: exerciseSets
        );

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var createdResult = result.Should().BeOfType<Created<IdResponse>>().Subject;
        createdResult.Value.Should().NotBeNull();
        createdResult.Value!.Id.Should().Be(exerciseId);

        await _sender
            .Received(1)
            .Send(
                Arg.Is<CreateExerciseCommand>(c =>
                    c.Name == "Test Exercise"
                    && c.MuscleGroups.Count == 2
                    && c.Difficulty == Difficulty.Medium
                    && c.PictureUrl == "https://example.com/picture.jpg"
                    && c.VideoUrl == "https://example.com/video.mp4"
                    && c.Description == "Test description"
                    && c.Equipment == "Barbell"
                    && c.Sets.Count == 2
                ),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandFails_ShouldReturnProblemDetails()
    {
        // Arrange
        var error = new Error("ValidationError", "Invalid exercise data");
        _sender
            .Send(Arg.Any<CreateExerciseCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<ExerciseId>(error));

        var request = new CreateExerciseRequest(
            Name: "Test Exercise",
            MuscleGroups: new List<string> { "Chest" },
            Difficulty: Difficulty.Easy,
            PictureUrl: null,
            VideoUrl: null,
            Description: null,
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
