using TrackYourLife.Modules.Trainings.Application.Features.Exercises.Commands.DeleteExercise;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.Exercises.Commands;

internal sealed record DeleteExerciseRequest(bool ForceDelete = false);

internal sealed class DeleteExercise(ISender sender) : Endpoint<DeleteExerciseRequest, IResult>
{
    public override void Configure()
    {
        Delete("{id}");
        Group<ExercisesGroup>();
        Description(x =>
            x.Produces(StatusCodes.Status204NoContent)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status403Forbidden)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        DeleteExerciseRequest req,
        CancellationToken ct
    )
    {
        return await Result
            .Create(new DeleteExerciseCommand(Route<ExerciseId>("id")!, req.ForceDelete))
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync();
    }
}
