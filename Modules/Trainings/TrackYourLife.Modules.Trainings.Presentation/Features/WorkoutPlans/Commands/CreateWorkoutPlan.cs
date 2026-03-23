using TrackYourLife.Modules.Trainings.Application.Features.WorkoutPlans.Commands.CreateWorkoutPlan;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.Modules.Trainings.Presentation.Contracts;
using TrackYourLife.SharedLib.Contracts.Shared;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.WorkoutPlans.Commands;

internal sealed record CreateWorkoutPlanRequest(
    string Name,
    bool IsActive,
    IReadOnlyList<TrainingId> TrainingIds
);

internal sealed class CreateWorkoutPlan(ISender sender)
    : Endpoint<CreateWorkoutPlanRequest, IResult>
{
    public override void Configure()
    {
        Post("");
        Group<WorkoutPlansGroup>();
        Description(x =>
            x.Produces<IdResponse>(StatusCodes.Status201Created)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
        );
    }

    public override async Task<IResult> ExecuteAsync(CreateWorkoutPlanRequest req, CancellationToken ct)
    {
        return await Result
            .Create(new CreateWorkoutPlanCommand(req.Name, req.IsActive, req.TrainingIds))
            .BindAsync(command => sender.Send(command, ct))
            .ToCreatedActionResultAsync(id => $"/{ApiRoutes.WorkoutPlans}/{id.Value}");
    }
}
