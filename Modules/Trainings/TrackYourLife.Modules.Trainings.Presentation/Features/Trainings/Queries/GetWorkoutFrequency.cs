using System.ComponentModel;
using TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetWorkoutFrequency;
using TrackYourLife.Modules.Trainings.Contracts.Dtos;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.Trainings.Queries;

internal sealed record GetWorkoutFrequencyRequest
{
    [QueryParam]
    public DateTime? StartDate { get; init; }

    [QueryParam]
    public DateTime? EndDate { get; init; }

    [QueryParam]
    [DefaultValue(OverviewType.Daily)]
    public OverviewType OverviewType { get; init; } = OverviewType.Daily;
}

internal sealed class GetWorkoutFrequency(ISender sender)
    : Endpoint<GetWorkoutFrequencyRequest, IResult>
{
    public override void Configure()
    {
        Get("frequency");
        Group<TrainingsGroup>();
        Description(x =>
            x.Produces<IReadOnlyList<WorkoutFrequencyDataDto>>(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        GetWorkoutFrequencyRequest req,
        CancellationToken ct
    )
    {
        return await Result
            .Create(new GetWorkoutFrequencyQuery(req.StartDate, req.EndDate, req.OverviewType))
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync(frequency => frequency.ToList());
    }
}
