using Microsoft.AspNetCore.Http.HttpResults;
using TrackYourLife.Modules.Trainings.Application.Features.ExercisesHistories.Queries.GetExerciseStats;
using TrackYourLife.Modules.Trainings.Contracts.Dtos;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.Modules.Trainings.Presentation.Features.ExercisesHistories.Queries;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Presentation.UnitTests.Features.ExercisesHistories.Queries;

public class GetExerciseStatsTests
{
    private readonly ISender _sender;
    private readonly GetExerciseStats _endpoint;

    public GetExerciseStatsTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new GetExerciseStats(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenQuerySucceeds_ShouldReturnOkWithStats()
    {
        var exerciseId = ExerciseId.NewId();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var dto = new ExerciseStatsDto(
            exerciseId,
            "Bench Press",
            ExerciseStatsRange.TwelveWeeks,
            today.AddDays(-7),
            today,
            ExerciseStatsChartMetric.Volume,
            true,
            true,
            new ExerciseStatsSummaryDto(10, 525, 1050, 2, 0),
            [new ExerciseImprovementTrendPointDto(today.AddDays(-1), 500)],
            [new ExerciseConsistencyPointDto(today, 2, 1)]
        );

        _sender
            .Send(Arg.Any<GetExerciseStatsQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success(dto)));

        var request = new GetExerciseStatsRequest
        {
            ExerciseId = exerciseId,
            Range = ExerciseStatsRange.TwelveWeeks,
            StartDate = today.AddDays(-7),
            EndDate = today,
            ChartMetric = ExerciseStatsChartMetric.MaxWeight,
        };

        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        var okResult = result.Should().BeOfType<Ok<ExerciseStatsDto>>().Subject;
        okResult.Value.Should().NotBeNull();
        okResult.Value!.ExerciseId.Should().Be(exerciseId);

        await _sender
            .Received(1)
            .Send(
                Arg.Is<GetExerciseStatsQuery>(q =>
                    q.ExerciseId == exerciseId
                    && q.Range == ExerciseStatsRange.TwelveWeeks
                    && q.StartDate == today.AddDays(-7)
                    && q.EndDate == today
                    && q.ChartMetric == ExerciseStatsChartMetric.MaxWeight
                ),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenQueryFails_ShouldReturnBadRequest()
    {
        var exerciseId = ExerciseId.NewId();
        _sender
            .Send(Arg.Any<GetExerciseStatsQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Failure<ExerciseStatsDto>(new Error("Error", "Failed"))));

        var request = new GetExerciseStatsRequest
        {
            ExerciseId = exerciseId,
            Range = ExerciseStatsRange.TwelveWeeks,
        };

        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
