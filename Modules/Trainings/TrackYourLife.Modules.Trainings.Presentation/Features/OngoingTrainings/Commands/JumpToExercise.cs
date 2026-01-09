using TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Commands.JumpToExercise;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.OngoingTrainings.Commands;

internal sealed record JumpToExerciseRequest(
    OngoingTrainingId OngoingTrainingId,
    int ExerciseIndex
);

internal sealed class JumpToExercise(ISender sender)
    : Endpoint<JumpToExerciseRequest, IResult>
{
    public override void Configure()
    {
        Put("active-training/jump-to-exercise");
        Group<OngoingTrainingsGroup>();
        Description(x =>
            x.Produces(StatusCodes.Status204NoContent)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        JumpToExerciseRequest req,
        CancellationToken ct
    )
    {
        return await Result
            .Create(new JumpToExerciseCommand(req.OngoingTrainingId, req.ExerciseIndex))
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync();
    }
}
