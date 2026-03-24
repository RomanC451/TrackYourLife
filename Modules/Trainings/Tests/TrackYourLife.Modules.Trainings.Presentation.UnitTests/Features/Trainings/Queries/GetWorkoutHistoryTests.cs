using Microsoft.AspNetCore.Http.HttpResults;
using TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetWorkoutHistory;
using TrackYourLife.Modules.Trainings.Contracts.Dtos;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.Modules.Trainings.Presentation.Features.Trainings.Queries;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Presentation.UnitTests.Features.Trainings.Queries;

public class GetWorkoutHistoryTests
{
    private readonly ISender _sender;
    private readonly GetWorkoutHistory _endpoint;

    public GetWorkoutHistoryTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new GetWorkoutHistory(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenQuerySucceeds_ShouldReturnOkWithHistory()
    {
        // Arrange
        var history = new List<WorkoutHistoryDto>
        {
            new(
                OngoingTrainingId.NewId(),
                TrainingId.NewId(),
                "Training A",
                DateTime.UtcNow.AddHours(-2),
                DateTime.UtcNow.AddHours(-1),
                3600,
                500,
                5,
                5
            ),
        };

        _sender
            .Send(Arg.Any<GetWorkoutHistoryQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success<IEnumerable<WorkoutHistoryDto>>(history)));

        var request = new GetWorkoutHistoryRequest
        {
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7)),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow),
        };

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Should().BeOfType<Ok<List<WorkoutHistoryDto>>>().Subject;
        okResult.Value.Should().NotBeNull();
        okResult.Value!.Should().HaveCount(1);

        await _sender
            .Received(1)
            .Send(Arg.Any<GetWorkoutHistoryQuery>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WhenQueryFails_ShouldReturnProblemDetails()
    {
        // Arrange
        var error = new Error("Error", "Failed to get workout history");

        _sender
            .Send(Arg.Any<GetWorkoutHistoryQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Failure<IEnumerable<WorkoutHistoryDto>>(error)));

        var request = new GetWorkoutHistoryRequest { StartDate = null, EndDate = null };

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
