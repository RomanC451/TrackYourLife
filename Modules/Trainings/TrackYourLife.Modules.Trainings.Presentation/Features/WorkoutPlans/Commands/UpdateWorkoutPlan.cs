using TrackYourLife.Modules.Trainings.Application.Features.WorkoutPlans.Commands.UpdateWorkoutPlan;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.Modules.Trainings.Domain.Features.WorkoutPlans;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.WorkoutPlans.Commands;

internal sealed record UpdateWorkoutPlanRequest(
    string Name,
    bool IsActive,
    IReadOnlyList<TrainingId> TrainingIds
);

internal sealed class UpdateWorkoutPlan(ISender sender)
    : Endpoint<UpdateWorkoutPlanRequest, IResult>
{
    public override void Configure()
    {
        Put("{id}");
        Group<WorkoutPlansGroup>();
        Description(x =>
            x.Produces(StatusCodes.Status204NoContent)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status403Forbidden)
        );
    }

    public override async Task<IResult> ExecuteAsync(UpdateWorkoutPlanRequest req, CancellationToken ct)
    {
        return await Result
            .Create(
                new UpdateWorkoutPlanCommand(
                    Route<WorkoutPlanId>("id")!,
                    req.Name,
                    req.IsActive,
                    req.TrainingIds
                )
            )
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync();
    }
}
