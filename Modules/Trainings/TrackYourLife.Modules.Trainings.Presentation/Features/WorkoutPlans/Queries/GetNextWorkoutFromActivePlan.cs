using TrackYourLife.Modules.Trainings.Application.Features.WorkoutPlans.Queries.GetNextWorkoutFromActivePlan;
using TrackYourLife.Modules.Trainings.Presentation.Features.Trainings;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.WorkoutPlans.Queries;

internal sealed class GetNextWorkoutFromActivePlan(ISender sender) : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Get("next-workout");
        Group<WorkoutPlansGroup>();
        Description(x =>
            x.Produces<TrainingDto>(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
        );
    }

    public override async Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        return await Result
            .Create(new GetNextWorkoutFromActivePlanQuery())
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync(training => training.ToDto());
    }
}
