using TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Queries.GetOngoingTrainingById;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.OngoingTrainings.Queries;

internal sealed class GetOngoingTrainingById(ISender sender) : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Get("{id}");
        Group<OngoingTrainingsGroup>();
        Description(x =>
            x.Produces<OngoingTrainingDto>(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
        );
    }

    public override async Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        return await Result
            .Create(new GetOngoingTrainingByIdQuery(Route<OngoingTrainingId>("id")!))
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync(ongoingTraining => ongoingTraining.ToDto());
    }
}
