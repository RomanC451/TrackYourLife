using TrackYourLife.Modules.Youtube.Presentation.Contracts;

namespace TrackYourLife.Modules.Youtube.Presentation.Features.DailyEntertainmentCounters;

internal sealed class DailyEntertainmentCountersGroup : Group
{
    public DailyEntertainmentCountersGroup()
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
