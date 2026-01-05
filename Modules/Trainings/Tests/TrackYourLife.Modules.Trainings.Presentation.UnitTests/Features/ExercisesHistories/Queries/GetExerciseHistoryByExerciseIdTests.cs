using Microsoft.AspNetCore.Http.HttpResults;
using TrackYourLife.Modules.Trainings.Application.Features.ExercisesHistories.Queries.GetExerciseHistoryByExerciseId;
using TrackYourLife.Modules.Trainings.Application.UnitTests.Utils;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.Modules.Trainings.Presentation.Features.ExercisesHistories.Models;
using TrackYourLife.Modules.Trainings.Presentation.Features.ExercisesHistories.Queries;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Presentation.UnitTests.Features.ExercisesHistories.Queries;

public class GetExerciseHistoryByExerciseIdTests
{
    private readonly ISender _sender;
    private readonly GetExerciseHistoryByExerciseId _endpoint;

    public GetExerciseHistoryByExerciseIdTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new GetExerciseHistoryByExerciseId(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenQuerySucceeds_ShouldReturnOkWithExerciseHistories()
    {
        // Arrange
        var exerciseId = ExerciseId.NewId();
        var exerciseHistories = new List<ExerciseHistoryReadModel>
        {
            ExerciseHistoryReadModelFaker.Generate(exerciseId: exerciseId),
            ExerciseHistoryReadModelFaker.Generate(exerciseId: exerciseId),
        };

        _sender
            .Send(Arg.Any<GetExerciseHistoryByExerciseIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success<IEnumerable<ExerciseHistoryReadModel>>(exerciseHistories)));

        var request = new GetExerciseHistoryByExerciseIdRequest
        {
            ExerciseId = exerciseId,
        };

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Should().BeOfType<Ok<List<ExerciseHistoryDto>>>().Subject;
        okResult.Value.Should().NotBeNull();
        okResult.Value!.Should().HaveCount(2);

        await _sender
            .Received(1)
            .Send(
                Arg.Is<GetExerciseHistoryByExerciseIdQuery>(q => q.ExerciseId == exerciseId),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenQueryFails_ShouldReturnProblemDetails()
    {
        // Arrange
        var exerciseId = ExerciseId.NewId();
        var error = new Error("Error", "Failed to get exercise history");
        _sender
            .Send(Arg.Any<GetExerciseHistoryByExerciseIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Failure<IEnumerable<ExerciseHistoryReadModel>>(error)));

        var request = new GetExerciseHistoryByExerciseIdRequest
        {
            ExerciseId = exerciseId,
        };

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
