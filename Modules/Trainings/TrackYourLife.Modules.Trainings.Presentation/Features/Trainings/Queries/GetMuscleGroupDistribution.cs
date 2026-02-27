using TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetMuscleGroupDistribution;
using TrackYourLife.Modules.Trainings.Contracts.Dtos;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.Trainings.Queries;

internal sealed record GetMuscleGroupDistributionRequest
{
    [QueryParam]
    public DateOnly? StartDate { get; init; }

    [QueryParam]
    public DateOnly? EndDate { get; init; }
}

internal sealed class GetMuscleGroupDistribution(ISender sender)
    : Endpoint<GetMuscleGroupDistributionRequest, IResult>
{
    public override void Configure()
    {
        Get("muscle-group-distribution");
        Group<TrainingsGroup>();
        Description(x =>
            x.Produces<IReadOnlyList<MuscleGroupDistributionDto>>(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        GetMuscleGroupDistributionRequest req,
        CancellationToken ct
    )
    {
        return await Result
            .Create(new GetMuscleGroupDistributionQuery(req.StartDate, req.EndDate))
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync(distribution => distribution.ToList());
    }
}
