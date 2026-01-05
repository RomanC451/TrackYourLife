using Microsoft.AspNetCore.Http.HttpResults;
using TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetTrainingsByUserId;
using TrackYourLife.Modules.Trainings.Application.UnitTests.Utils;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.Modules.Trainings.Presentation.Features.Trainings;
using TrackYourLife.Modules.Trainings.Presentation.Features.Trainings.Queries;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Presentation.UnitTests.Features.Trainings.Queries;

public class GetTrainingsTests
{
    private readonly ISender _sender;
    private readonly GetTrainings _endpoint;

    public GetTrainingsTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new GetTrainings(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenQuerySucceeds_ShouldReturnOkWithTrainings()
    {
        // Arrange
        var trainings = new List<TrainingReadModel>
        {
            TrainingReadModelFaker.Generate(name: "Training A"),
            TrainingReadModelFaker.Generate(name: "Training B"),
            TrainingReadModelFaker.Generate(name: "Training C"),
        };

        _sender
            .Send(Arg.Any<GetTrainingsByUserIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success<IEnumerable<TrainingReadModel>>(trainings)));

        // Act
        var result = await _endpoint.ExecuteAsync(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Should().BeOfType<Ok<List<TrainingDto>>>().Subject;
        okResult.Value.Should().NotBeNull();
        okResult.Value!.Should().HaveCount(3);
        okResult.Value.Should().BeInAscendingOrder(t => t.Name);

        await _sender
            .Received(1)
            .Send(Arg.Any<GetTrainingsByUserIdQuery>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WhenQueryFails_ShouldReturnProblemDetails()
    {
        // Arrange
        var error = new Error("Error", "Failed to get trainings");
        _sender
            .Send(Arg.Any<GetTrainingsByUserIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Failure<IEnumerable<TrainingReadModel>>(error)));

        // Act
        var result = await _endpoint.ExecuteAsync(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
