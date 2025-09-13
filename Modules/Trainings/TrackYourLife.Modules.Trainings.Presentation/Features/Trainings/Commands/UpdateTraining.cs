using TrackYourLife.Modules.Trainings.Application.Features.Trainings.Commands.UpdateTraining;
using TrackYourLife.Modules.Trainings.Domain.Core;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.Trainings.Commands;

internal sealed record UpdateTrainingRequest(
    string Name,
    List<string> MuscleGroups,
    Difficulty Difficulty,
    int Duration,
    int RestSeconds,
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
                    Name: req.Name,
                    MuscleGroups: req.MuscleGroups,
                    Difficulty: req.Difficulty,
                    Duration: req.Duration,
                    RestSeconds: req.RestSeconds,
                    Description: req.Description,
                    ExerciseIds: req.ExercisesIds
                )
            )
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync();
    }
}
