using System.ComponentModel;
using TrackYourLife.Modules.Trainings.Application.Features.ExercisesHistories.Queries.GetExerciseStats;
using TrackYourLife.Modules.Trainings.Contracts.Dtos;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.ExercisesHistories.Queries;

internal sealed record GetExerciseStatsRequest
{
    [QueryParam]
    public ExerciseId ExerciseId { get; init; } = ExerciseId.Empty;

    [QueryParam, DefaultValue(ExerciseStatsRange.TwelveWeeks)]
    public ExerciseStatsRange Range { get; init; } = ExerciseStatsRange.TwelveWeeks;

    [QueryParam]
    public DateOnly? StartDate { get; init; }

    [QueryParam]
    public DateOnly? EndDate { get; init; }

    [QueryParam, DefaultValue(ExerciseStatsChartMetric.Volume)]
    public ExerciseStatsChartMetric ChartMetric { get; init; } = ExerciseStatsChartMetric.Volume;
}

internal sealed class GetExerciseStats(ISender sender) : Endpoint<GetExerciseStatsRequest, IResult>
{
    public override void Configure()
    {
        Get("stats");
        Group<ExercisesHistoriesGroup>();
        Description(x =>
            x.Produces<ExerciseStatsDto>(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        GetExerciseStatsRequest request,
        CancellationToken ct
    )
    {
        return await Result
            .Create(
                new GetExerciseStatsQuery(
                    request.ExerciseId,
                    request.Range,
                    request.StartDate,
                    request.EndDate,
                    request.ChartMetric
                )
            )
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync(stats => stats);
    }
}
