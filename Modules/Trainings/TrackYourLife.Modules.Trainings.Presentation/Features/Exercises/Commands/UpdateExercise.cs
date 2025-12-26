using TrackYourLife.Modules.Trainings.Application.Features.Exercises.Commands.UpdateExercise;
using TrackYourLife.Modules.Trainings.Domain.Core;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.Exercises.Commands;

internal sealed record UpdateExerciseRequest(
    string Name,
    List<string> MuscleGroups,
    Difficulty Difficulty,
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
                    Id: Route<ExerciseId>("id")!,
                    Name: req.Name,
                    MuscleGroups: req.MuscleGroups,
                    Difficulty: req.Difficulty,
                    Description: req.Description,
                    VideoUrl: req.VideoUrl,
                    PictureUrl: req.PictureUrl,
                    Equipment: req.Equipment,
                    ExerciseSets: req.ExerciseSets.Select(x => x.EnsureHaveId()).ToList()
                )
            )
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync();
    }
}
