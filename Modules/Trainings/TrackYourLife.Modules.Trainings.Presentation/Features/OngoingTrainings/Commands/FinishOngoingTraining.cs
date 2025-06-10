using TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Commands.FinishOngoingTraining;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.OngoingTrainings.Commands;

internal sealed record FinishOngoingTrainingRequest(OngoingTrainingId Id);

internal sealed class FinishOngoingTraining(ISender sender)
    : Endpoint<FinishOngoingTrainingRequest, IResult>
{
    public override void Configure()
    {
        Put("active-training/finish");
        Group<OngoingTrainingsGroup>();
        Description(x =>
            x.Produces(StatusCodes.Status204NoContent)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        FinishOngoingTrainingRequest req,
        CancellationToken ct
    )
    {
        return await Result
            .Create(new FinishOngoingTrainingCommand(req.Id))
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync();
    }
}
