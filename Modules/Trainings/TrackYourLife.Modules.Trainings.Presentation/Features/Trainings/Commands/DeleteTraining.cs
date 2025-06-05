using TrackYourLife.Modules.Trainings.Application.Features.Trainings.Commands.DeleteTraining;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.Trainings.Commands;

internal sealed class DeleteTraining(ISender sender) : EndpointWithoutRequest<IResult>
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

    public override async Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        return await Result
            .Create(new DeleteTrainingCommand(Route<TrainingId>("id")!))
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync();
    }
}
