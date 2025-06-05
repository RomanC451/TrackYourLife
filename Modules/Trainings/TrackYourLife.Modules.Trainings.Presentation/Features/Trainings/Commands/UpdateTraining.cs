using TrackYourLife.Modules.Trainings.Application.Features.Trainings.Commands.UpdateTraining;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.Trainings.Commands;

internal sealed record UpdateTrainingRequest(
    string Name,
    int Duration,
    string? Description,
    IReadOnlyList<ExerciseId> ExercisesIds
);

internal sealed class UpdateTraining(ISender sender) : Endpoint<UpdateTrainingRequest, IResult>
{
    public override void Configure()
    {
        Put("{id}");
        Group<TrainingsGroup>();
        Description(x =>
            x.Produces(StatusCodes.Status204NoContent)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status403Forbidden)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        UpdateTrainingRequest req,
        CancellationToken ct
    )
    {
        return await Result
            .Create(
                new UpdateTrainingCommand(
                    Route<TrainingId>("id")!,
                    req.Name,
                    req.Duration,
                    req.Description,
                    req.ExercisesIds
                )
            )
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync();
    }
}
