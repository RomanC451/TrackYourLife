using TrackYourLife.Modules.Youtube.Presentation.Contracts;

namespace TrackYourLife.Modules.Youtube.Presentation.Features.DailyCategoryWatchCounters;

internal sealed class DailyCategoryWatchCountersGroup : Group
{
    public DailyCategoryWatchCountersGroup()
    {
        Configure(
            ApiRoutes.Settings,
            ep =>
            {
                ep.Description(x => x.ProducesProblem(StatusCodes.Status401Unauthorized));
            }
        );
    }
}
