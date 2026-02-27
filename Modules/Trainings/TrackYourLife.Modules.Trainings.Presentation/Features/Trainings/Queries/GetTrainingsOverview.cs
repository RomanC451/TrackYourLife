using TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetTrainingsOverview;
using TrackYourLife.Modules.Trainings.Contracts.Dtos;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.Trainings.Queries;

internal sealed record GetTrainingsOverviewRequest
{
    [QueryParam]
    public DateOnly? StartDate { get; init; }

    [QueryParam]
    public DateOnly? EndDate { get; init; }
}

internal sealed class GetTrainingsOverview(ISender sender)
    : Endpoint<GetTrainingsOverviewRequest, IResult>
{
    public override void Configure()
    {
        Get("overview");
        Group<TrainingsGroup>();
        Description(x =>
            x.Produces<TrainingsOverviewDto>(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        GetTrainingsOverviewRequest req,
        CancellationToken ct
    )
    {
        return await Result
            .Create(new GetTrainingsOverviewQuery(req.StartDate, req.EndDate))
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync(overview => overview);
    }
}
