using TrackYourLife.Modules.Trainings.Application.Features.WorkoutPlans.Commands.DeleteWorkoutPlan;
using TrackYourLife.Modules.Trainings.Domain.Features.WorkoutPlans;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.WorkoutPlans.Commands;

internal sealed class DeleteWorkoutPlan(ISender sender) : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Delete("{id}");
        Group<WorkoutPlansGroup>();
        Description(x =>
            x.Produces(StatusCodes.Status204NoContent)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status403Forbidden)
        );
    }

    public override async Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        return await Result
            .Create(new DeleteWorkoutPlanCommand(Route<WorkoutPlanId>("id")!))
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync();
    }
}
