using Microsoft.AspNetCore.Http.HttpResults;
using TrackYourLife.Modules.Trainings.Application.Features.Exercises.Queries.GetExercisesByUserId;
using TrackYourLife.Modules.Trainings.Application.UnitTests.Utils;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Presentation.Features.Exercises.Models;
using TrackYourLife.Modules.Trainings.Presentation.Features.Exercises.Queries;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Presentation.UnitTests.Features.Exercises.Queries;

public class GetExercisesTests
{
    private readonly ISender _sender;
    private readonly GetExercises _endpoint;

    public GetExercisesTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new GetExercises(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenQuerySucceeds_ShouldReturnOkWithExercises()
    {
        // Arrange
        var exercises = new List<ExerciseReadModel>
        {
            ExerciseReadModelFaker.Generate(name: "Exercise A"),
            ExerciseReadModelFaker.Generate(name: "Exercise B"),
            ExerciseReadModelFaker.Generate(name: "Exercise C"),
        };

        _sender
            .Send(Arg.Any<GetExercisesByUserIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success<IEnumerable<ExerciseReadModel>>(exercises)));

        // Act
        var result = await _endpoint.ExecuteAsync(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Should().BeOfType<Ok<List<ExerciseDto>>>().Subject;
        okResult.Value.Should().NotBeNull();
        okResult.Value!.Should().HaveCount(3);
        okResult.Value.Should().BeInAscendingOrder(e => e.Name);

        await _sender
            .Received(1)
            .Send(Arg.Any<GetExercisesByUserIdQuery>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WhenQueryFails_ShouldReturnProblemDetails()
    {
        // Arrange
        var error = new Error("Error", "Failed to get exercises");
        _sender
            .Send(Arg.Any<GetExercisesByUserIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Failure<IEnumerable<ExerciseReadModel>>(error)));

        // Act
        var result = await _endpoint.ExecuteAsync(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
