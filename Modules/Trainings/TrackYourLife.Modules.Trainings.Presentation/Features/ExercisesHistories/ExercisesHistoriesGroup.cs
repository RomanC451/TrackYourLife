using TrackYourLife.Modules.Trainings.Presentation.Contracts;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.ExercisesHistories;

internal sealed class ExercisesHistoriesGroup : Group
{
    public ExercisesHistoriesGroup()
    {
        Configure(
            ApiRoutes.ExercisesHistories,
            ep =>
            {
                ep.Description(x => x.ProducesProblem(StatusCodes.Status401Unauthorized));
            }
        );
    }
}
