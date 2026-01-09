using TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Queries.GetOngoingTrainingByUserId;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.OngoingTrainings.Queries;

public class GetActiveOngoingTraining(ISender sender) : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Get("active-training");
        Group<OngoingTrainingsGroup>();
        Description(x =>
            x.Produces<OngoingTrainingDto>(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
        );
    }

    public override async Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        return await Result
            .Create(new GetOngoingTrainingByUserIdQuery())
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync(result => result.OngoingTraining.ToDto(result.ExerciseHistories));
    }
}
