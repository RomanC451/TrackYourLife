using TrackYourLife.Modules.Trainings.Application.Features.Exercises.Commands.UpdateExercise;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.Sets;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.Exercises.Commands;

internal sealed record UpdateExerciseRequest(
    string Name,
    string? Description,
    string? VideoUrl,
    string? PictureUrl,
    string? Equipment,
    List<ExerciseSet> ExerciseSets
);

internal sealed class UpdateExercise(ISender sender) : Endpoint<UpdateExerciseRequest, IResult>
{
    public override void Configure()
    {
        Put("{id}");
        Group<ExercisesGroup>();
        Description(x =>
            x.Produces(StatusCodes.Status204NoContent)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status403Forbidden)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        UpdateExerciseRequest req,
        CancellationToken ct
    )
    {
        return await Result
            .Create(
                new UpdateExerciseCommand(
                    Route<ExerciseId>("id")!,
                    req.Name,
                    req.Description,
                    req.VideoUrl,
                    req.PictureUrl,
                    req.Equipment,
                    req.ExerciseSets
                )
            )
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync();
    }
}
