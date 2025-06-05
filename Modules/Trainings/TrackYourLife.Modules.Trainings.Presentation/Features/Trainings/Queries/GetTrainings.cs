using TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetTrainingsByUserId;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.Trainings.Queries;

public sealed class GetTrainings(ISender sender) : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Get("");
        Group<TrainingsGroup>();
        Description(x => x.Produces<List<TrainingDto>>(StatusCodes.Status200OK));
    }

    public override async Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        return await Result
            .Create(new GetTrainingsByUserIdQuery())
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync(trainings => trainings.Select(t => t.ToDto()).ToList());
    }
}
