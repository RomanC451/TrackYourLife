using TrackYourLife.Modules.Trainings.Presentation.Contracts;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.OngoingTrainings;

internal sealed class OngoingTrainingsGroup : Group
{
    public OngoingTrainingsGroup()
    {
        Configure(
            ApiRoutes.OngoingTrainings,
            ep =>
            {
                ep.Description(x => x.ProducesProblem(StatusCodes.Status401Unauthorized));
            }
        );
    }
}
