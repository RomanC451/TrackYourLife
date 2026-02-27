using System.ComponentModel;
using TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetWorkoutDurationHistory;
using TrackYourLife.Modules.Trainings.Contracts.Dtos;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.Trainings.Queries;

internal sealed record GetWorkoutDurationHistoryRequest
{
    [QueryParam]
    public DateOnly? StartDate { get; init; }

    [QueryParam]
    public DateOnly? EndDate { get; init; }

    [QueryParam]
    [DefaultValue(OverviewType.Daily)]
    public OverviewType OverviewType { get; init; } = OverviewType.Daily;

    [QueryParam]
    [DefaultValue(AggregationType.Sum)]
    public AggregationType AggregationType { get; init; } = AggregationType.Sum;
}

internal sealed class GetWorkoutDurationHistory(ISender sender)
    : Endpoint<GetWorkoutDurationHistoryRequest, IResult>
{
    public override void Configure()
    {
        Get("duration");
        Group<TrainingsGroup>();
        Description(x =>
            x.Produces<IReadOnlyList<WorkoutAggregatedValueDto>>(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        GetWorkoutDurationHistoryRequest req,
        CancellationToken ct
    )
    {
        return await Result
            .Create(
                new GetWorkoutDurationHistoryQuery(
                    req.StartDate,
                    req.EndDate,
                    req.OverviewType,
                    req.AggregationType
                )
            )
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync(data => data.ToList());
    }
}
