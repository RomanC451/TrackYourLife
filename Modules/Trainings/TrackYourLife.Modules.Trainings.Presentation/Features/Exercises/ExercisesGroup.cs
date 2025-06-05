using TrackYourLife.Modules.Trainings.Presentation.Contracts;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.Exercises;

internal sealed class ExercisesGroup : Group
{
    public ExercisesGroup()
    {
        Configure(
            ApiRoutes.Exercises,
            ep =>
            {
                ep.Description(x => x.ProducesProblem(StatusCodes.Status401Unauthorized));
            }
        );
    }
}
