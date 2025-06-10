using TrackYourLife.Modules.Trainings.Application.Features.Trainings.Commands.DeleteTraining;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.Trainings.Commands;

internal sealed record DeleteTrainingRequest(bool Force = false);

internal sealed class DeleteTraining(ISender sender) : Endpoint<DeleteTrainingRequest, IResult>
{
    public override void Configure()
    {
        Delete("{id}");
        Group<TrainingsGroup>();
        Description(x =>
            x.Produces(StatusCodes.Status204NoContent)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status403Forbidden)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        DeleteTrainingRequest request,
        CancellationToken ct
    )
    {
        return await Result
            .Create(new DeleteTrainingCommand(Route<TrainingId>("id")!, request.Force))
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync();
    }
}
