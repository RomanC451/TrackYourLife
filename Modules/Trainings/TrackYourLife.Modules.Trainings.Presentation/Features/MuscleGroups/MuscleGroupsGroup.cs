using TrackYourLife.Modules.Trainings.Presentation.Contracts;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.MuscleGroups;

internal sealed class MuscleGroupsGroup : Group
{
    public MuscleGroupsGroup()
    {
        Configure(
            ApiRoutes.MuscleGroups,
            ep =>
            {
                ep.Description(x => x.ProducesProblem(StatusCodes.Status401Unauthorized));
            }
        );
    }
}
