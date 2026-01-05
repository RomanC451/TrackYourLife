using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using TrackYourLife.Modules.Trainings.Application.Features.Exercises.Queries.GetExerciseById;
using TrackYourLife.Modules.Trainings.Application.UnitTests.Utils;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Presentation.Features.Exercises.Models;
using TrackYourLife.Modules.Trainings.Presentation.Features.Exercises.Queries;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Presentation.UnitTests;

namespace TrackYourLife.Modules.Trainings.Presentation.UnitTests.Features.Exercises.Queries;

public class GetExerciseByIdTests
{
    private readonly ISender _sender;
    private readonly GetExerciseById _endpoint;

    public GetExerciseByIdTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new GetExerciseById(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenQuerySucceeds_ShouldReturnOkWithExercise()
    {
        // Arrange
        var exerciseId = ExerciseId.NewId();
        var exercise = ExerciseReadModelFaker.Generate(id: exerciseId);

        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues = new RouteValueDictionary { { "id", exerciseId.Value } };
        _endpoint.SetHttpContext(httpContext);

        _sender
            .Send(Arg.Any<GetExerciseByIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(exercise));

        var request = new GetExerciseByIdRequest(exerciseId);

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Should().BeOfType<Ok<ExerciseDto>>().Subject;
        okResult.Value.Should().NotBeNull();
        okResult.Value!.Id.Should().Be(exerciseId);
        okResult.Value.Name.Should().Be(exercise.Name);

        await _sender
            .Received(1)
            .Send(
                Arg.Is<GetExerciseByIdQuery>(q => q.Id == exerciseId),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenQueryFails_ShouldReturnProblemDetails()
    {
        // Arrange
        var exerciseId = ExerciseId.NewId();
        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues = new RouteValueDictionary { { "id", exerciseId.Value } };
        _endpoint.SetHttpContext(httpContext);

        var error = new Error("NotFound", "Exercise not found");
        _sender
            .Send(Arg.Any<GetExerciseByIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<ExerciseReadModel>(error));

        var request = new GetExerciseByIdRequest(exerciseId);

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
