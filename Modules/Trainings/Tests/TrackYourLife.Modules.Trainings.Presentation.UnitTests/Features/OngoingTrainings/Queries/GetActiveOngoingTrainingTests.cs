using Microsoft.AspNetCore.Http.HttpResults;
using TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Queries.GetOngoingTrainingByUserId;
using TrackYourLife.Modules.Trainings.Application.UnitTests.Utils;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.Modules.Trainings.Domain.Features.TrainingExercises;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.Modules.Trainings.Presentation.Features.OngoingTrainings;
using TrackYourLife.Modules.Trainings.Presentation.Features.OngoingTrainings.Queries;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Presentation.UnitTests.Features.OngoingTrainings.Queries;

public class GetActiveOngoingTrainingTests
{
    private readonly ISender _sender;
    private readonly GetActiveOngoingTraining _endpoint;

    public GetActiveOngoingTrainingTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new GetActiveOngoingTraining(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenQuerySucceeds_ShouldReturnOkWithOngoingTraining()
    {
        // Arrange
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
            training: training,
            exerciseIndex: 0,
            setIndex: 0
        );

        _sender
            .Send(Arg.Any<GetOngoingTrainingByUserIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success(ongoingTraining)));

        // Act
        var result = await _endpoint.ExecuteAsync(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Should().BeOfType<Ok<OngoingTrainingDto>>().Subject;
        okResult.Value.Should().NotBeNull();
        okResult.Value!.Id.Should().Be(ongoingTraining.Id);
        okResult.Value.Training.Should().NotBeNull();

        await _sender
            .Received(1)
            .Send(Arg.Any<GetOngoingTrainingByUserIdQuery>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WhenQueryFails_ShouldReturnProblemDetails()
    {
        // Arrange
        var error = new Error("NotFound", "No active ongoing training found");
        _sender
            .Send(Arg.Any<GetOngoingTrainingByUserIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Failure<OngoingTrainingReadModel>(error)));

        // Act
        var result = await _endpoint.ExecuteAsync(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
