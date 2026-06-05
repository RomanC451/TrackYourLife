using System.ComponentModel;
using TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetTrainingStats;
using TrackYourLife.Modules.Trainings.Contracts.Dtos;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.Trainings.Queries;

internal sealed record GetTrainingStatsRequest
{
    [QueryParam, DefaultValue(ExerciseStatsRange.TwelveWeeks)]
    public ExerciseStatsRange Range { get; init; } = ExerciseStatsRange.TwelveWeeks;

    [QueryParam]
    public DateOnly? StartDate { get; init; }

    [QueryParam]
    public DateOnly? EndDate { get; init; }

    [QueryParam, DefaultValue(AggregationType.Sum)]
    public AggregationType ChartAggregationType { get; init; } = AggregationType.Sum;
}

internal sealed class GetTrainingStats(ISender sender) : Endpoint<GetTrainingStatsRequest, IResult>
{
    public override void Configure()
    {
        Get("{trainingId}/stats");
        Group<TrainingsGroup>();
        Description(x =>
            x.Produces<TrainingStatsDto>(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status403Forbidden)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        GetTrainingStatsRequest request,
        CancellationToken ct
    )
    {
        return await Result
            .Create(
                new GetTrainingStatsQuery(
                    Route<TrainingId>("trainingId")!,
                    request.Range,
                    request.StartDate,
                    request.EndDate,
                    request.ChartAggregationType
                )
            )
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync(stats => stats);
    }
}
