using Microsoft.AspNetCore.Http.HttpResults;
using TrackYourLife.Modules.Trainings.Application.Features.Trainings.Commands.CreateTraining;
using TrackYourLife.Modules.Trainings.Domain.Core;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.Modules.Trainings.Presentation.Features.Trainings.Commands;
using TrackYourLife.SharedLib.Contracts.Shared;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Presentation.UnitTests.Features.Trainings.Commands;

public class CreateTrainingTests
{
    private readonly ISender _sender;
    private readonly CreateTraining _endpoint;

    public CreateTrainingTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new CreateTraining(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandSucceeds_ShouldReturnCreatedWithIdResponse()
    {
        // Arrange
        var trainingId = TrainingId.NewId();
        var exerciseId1 = ExerciseId.NewId();
        var exerciseId2 = ExerciseId.NewId();

        _sender
            .Send(Arg.Any<CreateTrainingCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(trainingId));

        var request = new CreateTrainingRequest(
            Name: "Test Training",
            MuscleGroups: new List<string> { "Chest", "Triceps" },
            Difficulty: Difficulty.Medium,
            ExercisesIds: new List<ExerciseId> { exerciseId1, exerciseId2 },
            Description: "Test description",
            Duration: 60,
            RestSeconds: 90
        );

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var createdResult = result.Should().BeOfType<Created<IdResponse>>().Subject;
        createdResult.Value.Should().NotBeNull();
        createdResult.Value!.Id.Should().Be(trainingId);

        await _sender
            .Received(1)
            .Send(
                Arg.Is<CreateTrainingCommand>(c =>
                    c.Name == "Test Training"
                    && c.MuscleGroups.Count == 2
                    && c.Difficulty == Difficulty.Medium
                    && c.ExercisesIds.Count == 2
                    && c.Description == "Test description"
                    && c.Duration == 60
                    && c.RestSeconds == 90
                ),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandFails_ShouldReturnProblemDetails()
    {
        // Arrange
        var error = new Error("ValidationError", "Invalid training data");
        _sender
            .Send(Arg.Any<CreateTrainingCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<TrainingId>(error));

        var request = new CreateTrainingRequest(
            Name: "Test Training",
            MuscleGroups: new List<string> { "Chest" },
            Difficulty: Difficulty.Easy,
            ExercisesIds: new List<ExerciseId>(),
            Description: null,
            Duration: 0,
            RestSeconds: 0
        );

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
