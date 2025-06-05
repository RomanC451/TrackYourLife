using TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Commands.NextOngoingTraining;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.OngoingTrainings.Commands;

internal sealed record NextOngoingTrainingRequest(OngoingTrainingId OngoingTrainingId);

internal sealed class NextOngoingTraining(ISender sender)
    : Endpoint<NextOngoingTrainingRequest, IResult>
{
    public override void Configure()
    {
        Put("active-training/next");
        Group<OngoingTrainingsGroup>();
        Description(x =>
            x.Produces(StatusCodes.Status204NoContent)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        NextOngoingTrainingRequest req,
        CancellationToken ct
    )
    {
        return await Result
            .Create(new NextOngoingTrainingCommand(req.OngoingTrainingId))
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync();
    }
}
