using TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetTrainingTemplatesUsage;
using TrackYourLife.Modules.Trainings.Contracts.Dtos;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.Trainings.Queries;

internal sealed record GetTrainingTemplatesUsageRequest
{
    [QueryParam]
    public DateTime? StartDate { get; init; }

    [QueryParam]
    public DateTime? EndDate { get; init; }
}

internal sealed class GetTrainingTemplatesUsage(ISender sender)
    : Endpoint<GetTrainingTemplatesUsageRequest, IResult>
{
    public override void Configure()
    {
        Get("templates-usage");
        Group<TrainingsGroup>();
        Description(x =>
            x.Produces<IEnumerable<TrainingTemplateUsageDto>>(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        GetTrainingTemplatesUsageRequest req,
        CancellationToken ct
    )
    {
        return await Result
            .Create(new GetTrainingTemplatesUsageQuery(req.StartDate, req.EndDate))
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync(templates => templates.ToList());
    }
}
