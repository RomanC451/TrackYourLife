using TrackYourLife.Modules.Trainings.Presentation.Contracts;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.WorkoutPlans;

internal sealed class WorkoutPlansGroup : Group
{
    public WorkoutPlansGroup()
    {
        Configure(
            ApiRoutes.WorkoutPlans,
            ep =>
            {
                ep.Description(x => x.ProducesProblem(StatusCodes.Status401Unauthorized));
            }
        );
    }
}
