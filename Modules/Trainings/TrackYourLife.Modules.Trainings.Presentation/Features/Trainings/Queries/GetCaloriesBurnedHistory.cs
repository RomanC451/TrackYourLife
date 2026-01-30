using System.ComponentModel;
using TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetCaloriesBurnedHistory;
using TrackYourLife.Modules.Trainings.Contracts.Dtos;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.Trainings.Queries;

internal sealed record GetCaloriesBurnedHistoryRequest
{
    [QueryParam]
    public DateTime? StartDate { get; init; }

    [QueryParam]
    public DateTime? EndDate { get; init; }

    [QueryParam]
    [DefaultValue(OverviewType.Daily)]
    public OverviewType OverviewType { get; init; } = OverviewType.Daily;

    [QueryParam]
    [DefaultValue(AggregationType.Sum)]
    public AggregationType AggregationType { get; init; } = AggregationType.Sum;
}

internal sealed class GetCaloriesBurnedHistory(ISender sender)
    : Endpoint<GetCaloriesBurnedHistoryRequest, IResult>
{
    public override void Configure()
    {
        Get("calories");
        Group<TrainingsGroup>();
        Description(x =>
            x.Produces<IReadOnlyList<WorkoutAggregatedValueDto>>(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        GetCaloriesBurnedHistoryRequest req,
        CancellationToken ct
    )
    {
        return await Result
            .Create(
                new GetCaloriesBurnedHistoryQuery(
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
