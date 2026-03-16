using System.ComponentModel;
using TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetWorkoutHistory;
using TrackYourLife.Modules.Trainings.Contracts.Dtos;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.Trainings.Queries;

internal sealed record GetWorkoutHistoryRequest
{
    [QueryParam]
    public DateOnly? StartDate { get; init; }

    [QueryParam]
    public DateOnly? EndDate { get; init; }
}

internal sealed class GetWorkoutHistory(ISender sender)
    : Endpoint<GetWorkoutHistoryRequest, IResult>
{
    public override void Configure()
    {
        Get("workout-history");
        Group<TrainingsGroup>();
        Description(x =>
            x.Produces<IReadOnlyList<WorkoutHistoryDto>>(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        GetWorkoutHistoryRequest req,
        CancellationToken ct
    )
    {
        return await Result
            .Create(new GetWorkoutHistoryQuery(req.StartDate, req.EndDate))
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync(history => history.ToList());
    }
}
