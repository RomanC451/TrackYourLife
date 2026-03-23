using TrackYourLife.Modules.Trainings.Application.Features.WorkoutPlans.Queries.GetActiveWorkoutPlan;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.WorkoutPlans.Queries;

internal sealed class GetActiveWorkoutPlan(ISender sender) : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Get("active");
        Group<WorkoutPlansGroup>();
        Description(x =>
            x.Produces<WorkoutPlanDto>(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
        );
    }

    public override async Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        return await Result
            .Create(new GetActiveWorkoutPlanQuery())
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync(workoutPlan => workoutPlan.ToDto());
    }
}
