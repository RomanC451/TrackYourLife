using TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Commands.SkipExercise;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.OngoingTrainings.Commands;

internal sealed record SkipExerciseRequest(OngoingTrainingId OngoingTrainingId);

internal sealed class SkipExercise(ISender sender)
    : Endpoint<SkipExerciseRequest, IResult>
{
    public override void Configure()
    {
        Put("active-training/skip");
        Group<OngoingTrainingsGroup>();
        Description(x =>
            x.Produces(StatusCodes.Status204NoContent)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        SkipExerciseRequest req,
        CancellationToken ct
    )
    {
        return await Result
            .Create(new SkipExerciseCommand(req.OngoingTrainingId))
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync();
    }
}
