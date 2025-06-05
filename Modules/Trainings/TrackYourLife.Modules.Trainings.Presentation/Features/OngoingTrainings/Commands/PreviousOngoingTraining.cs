using TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Commands.PreviousOngoingTraining;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.OngoingTrainings.Commands;

internal sealed record PreviousOngoingTrainingRequest(OngoingTrainingId OngoingTrainingId);

internal sealed class PreviousOngoingTraining(ISender sender)
    : Endpoint<PreviousOngoingTrainingRequest, IResult>
{
    public override void Configure()
    {
        Put("active-training/previous");
        Group<OngoingTrainingsGroup>();
        Description(x =>
            x.Produces(StatusCodes.Status204NoContent)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        PreviousOngoingTrainingRequest req,
        CancellationToken ct
    )
    {
        return await Result
            .Create(new PreviousOngoingTrainingCommand(req.OngoingTrainingId))
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync();
    }
}
