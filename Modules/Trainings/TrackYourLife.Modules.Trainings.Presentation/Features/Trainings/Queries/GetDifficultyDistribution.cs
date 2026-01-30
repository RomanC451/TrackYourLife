using TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetDifficultyDistribution;
using TrackYourLife.Modules.Trainings.Contracts.Dtos;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.Trainings.Queries;

internal sealed record GetDifficultyDistributionRequest
{
    [QueryParam]
    public DateTime? StartDate { get; init; }

    [QueryParam]
    public DateTime? EndDate { get; init; }
}

internal sealed class GetDifficultyDistribution(ISender sender)
    : Endpoint<GetDifficultyDistributionRequest, IResult>
{
    public override void Configure()
    {
        Get("difficulty-distribution");
        Group<TrainingsGroup>();
        Description(x =>
            x.Produces<IReadOnlyList<DifficultyDistributionDto>>(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        GetDifficultyDistributionRequest req,
        CancellationToken ct
    )
    {
        return await Result
            .Create(new GetDifficultyDistributionQuery(req.StartDate, req.EndDate))
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync(distribution => distribution.ToList());
    }
}
