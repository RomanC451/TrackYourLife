using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Queries.GetOngoingTrainingById;
using TrackYourLife.Modules.Trainings.Application.UnitTests.Utils;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.Modules.Trainings.Domain.Features.TrainingExercises;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.Modules.Trainings.Presentation.Features.OngoingTrainings;
using TrackYourLife.Modules.Trainings.Presentation.Features.OngoingTrainings.Queries;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Presentation.UnitTests;

namespace TrackYourLife.Modules.Trainings.Presentation.UnitTests.Features.OngoingTrainings.Queries;

public class GetOngoingTrainingByIdTests
{
    private readonly ISender _sender;
    private readonly GetOngoingTrainingById _endpoint;

    public GetOngoingTrainingByIdTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new GetOngoingTrainingById(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenQuerySucceeds_ShouldReturnOkWithOngoingTraining()
    {
        // Arrange
        var ongoingTrainingId = OngoingTrainingId.NewId();
        var trainingId = TrainingId.NewId();
        var userId = UserId.NewId();
        var exercise1 = ExerciseReadModelFaker.Generate();
        var exercise2 = ExerciseReadModelFaker.Generate();

        var trainingExercise1 = new TrainingExerciseReadModel(trainingId, exercise1.Id, 0);
        var trainingExercise2 = new TrainingExerciseReadModel(trainingId, exercise2.Id, 1);
        trainingExercise1.Exercise = exercise1;
        trainingExercise2.Exercise = exercise2;

        var training = TrainingReadModelFaker.Generate(id: trainingId, userId: userId);
        training = training with
        {
            TrainingExercises = new List<TrainingExerciseReadModel>
            {
                trainingExercise1,
                trainingExercise2,
            },
        };

        var ongoingTraining = OngoingTrainingReadModelFaker.Generate(
            id: ongoingTrainingId,
            training: training,
            exerciseIndex: 0,
            setIndex: 0
        );

        var exerciseHistories = new List<ExerciseHistoryReadModel>
        {
            ExerciseHistoryReadModelFaker.Generate(ongoingTrainingId: ongoingTrainingId),
        };

        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues = new RouteValueDictionary
        {
            { "id", ongoingTrainingId.Value },
        };
        _endpoint.SetHttpContext(httpContext);

        _sender
            .Send(Arg.Any<GetOngoingTrainingByIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success((ongoingTraining, (IEnumerable<ExerciseHistoryReadModel>)exerciseHistories))));

        // Act
        var result = await _endpoint.ExecuteAsync(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Should().BeOfType<Ok<OngoingTrainingDto>>().Subject;
        okResult.Value.Should().NotBeNull();
        okResult.Value!.Id.Should().Be(ongoingTrainingId);
        okResult.Value.Training.Should().NotBeNull();

        await _sender
            .Received(1)
            .Send(
                Arg.Is<GetOngoingTrainingByIdQuery>(q => q.Id == ongoingTrainingId),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenQueryFails_ShouldReturnProblemDetails()
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
            .Send(Arg.Any<GetOngoingTrainingByIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Failure<(OngoingTrainingReadModel OngoingTraining, IEnumerable<ExerciseHistoryReadModel> ExerciseHistories)>(error)));

        // Act
        var result = await _endpoint.ExecuteAsync(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
