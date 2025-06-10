using TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Commands.AdjustExerciseSets;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.OngoingTrainings.Commands;

internal sealed record AdjustExerciseSetsRequest(
    ExerciseId ExerciseId,
    List<ExerciseSetChange> ExerciseSetChanges
);

internal sealed class AdjustExerciseSets(ISender sender)
    : Endpoint<AdjustExerciseSetsRequest, IResult>
{
    public override void Configure()
    {
        Put("{id}/adjust-sets");
        Group<OngoingTrainingsGroup>();
    }

    public override async Task<IResult> ExecuteAsync(
        AdjustExerciseSetsRequest request,
        CancellationToken ct
    )
    {
        return await Result
            .Create(
                new AdjustExerciseSetsCommand(
                    Route<OngoingTrainingId>("id")!,
                    request.ExerciseId,
                    request.ExerciseSetChanges
                )
            )
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync();
    }
}
