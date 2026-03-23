using TrackYourLife.Modules.Trainings.Application.Features.WorkoutPlans.Queries.GetWorkoutPlans;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.WorkoutPlans.Queries;

internal sealed class GetWorkoutPlans(ISender sender) : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Get("");
        Group<WorkoutPlansGroup>();
        Description(x => x.Produces<List<WorkoutPlanDto>>(StatusCodes.Status200OK));
    }

    public override async Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        return await Result
            .Create(new GetWorkoutPlansQuery())
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync(workoutPlans => workoutPlans.Select(wp => wp.ToDto()).ToList());
    }
}
