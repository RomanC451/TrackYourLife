using TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Commands.UpdateOngoingTraining;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.OngoingTrainings.Commands;

internal sealed record UpdateOngoingTrainingRequest(int CaloriesBurned, int DurationMinutes);

internal sealed class UpdateOngoingTraining(ISender sender)
    : Endpoint<UpdateOngoingTrainingRequest, IResult>
{
    public override void Configure()
    {
        Put("{id}");
        Group<OngoingTrainingsGroup>();
        Description(x =>
            x.Produces(StatusCodes.Status204NoContent)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        UpdateOngoingTrainingRequest req,
        CancellationToken ct
    )
    {
        return await Result
            .Create(
                new UpdateOngoingTrainingCommand(
                    Route<OngoingTrainingId>("id")!,
                    req.CaloriesBurned,
                    req.DurationMinutes
                )
            )
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync();
    }
}
