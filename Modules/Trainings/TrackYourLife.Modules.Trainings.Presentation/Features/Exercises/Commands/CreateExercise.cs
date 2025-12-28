using TrackYourLife.Modules.Trainings.Application.Features.Exercises.Commands.CreateExercise;
using TrackYourLife.Modules.Trainings.Domain.Core;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Presentation.Contracts;
using TrackYourLife.SharedLib.Contracts.Shared;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.Exercises.Commands;

internal sealed record CreateExerciseRequest(
    string Name,
    List<string> MuscleGroups,
    Difficulty Difficulty,
    string? PictureUrl,
    string? VideoUrl,
    string? Description,
    string? Equipment,
    List<ExerciseSet> ExerciseSets
);

internal sealed class CreateExercise(ISender sender) : Endpoint<CreateExerciseRequest, IResult>
{
    public override void Configure()
    {
        Post("");
        Group<ExercisesGroup>();
        Description(x =>
            x.Produces<IdResponse>(StatusCodes.Status201Created)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        CreateExerciseRequest req,
        CancellationToken ct
    )
    {
        return await Result
            .Create(
                new CreateExerciseCommand(
                    Name: req.Name,
                    MuscleGroups: req.MuscleGroups,
                    Difficulty: req.Difficulty,
                    PictureUrl: req.PictureUrl,
                    VideoUrl: req.VideoUrl,
                    Description: req.Description,
                    Equipment: req.Equipment,
                    Sets: req.ExerciseSets
                )
            )
            .BindAsync(command => sender.Send(command, ct))
            .ToCreatedActionResultAsync(exerciseId => $"/{ApiRoutes.Exercises}/{exerciseId.Value}");
    }
}
