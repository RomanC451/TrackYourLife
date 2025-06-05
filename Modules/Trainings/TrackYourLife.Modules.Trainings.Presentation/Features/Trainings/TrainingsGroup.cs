using TrackYourLife.Modules.Trainings.Presentation.Contracts;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.Trainings;

internal sealed class TrainingsGroup : Group
{
    public TrainingsGroup()
    {
        Configure(
            ApiRoutes.Trainings,
            ep =>
            {
                ep.Description(x => x.ProducesProblem(StatusCodes.Status401Unauthorized));
            }
        );
    }
}
