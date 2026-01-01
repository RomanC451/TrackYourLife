using TrackYourLife.Modules.Youtube.Presentation.Contracts;

namespace TrackYourLife.Modules.Youtube.Presentation.Features.DailyDivertissmentCounters;

internal sealed class DailyDivertissmentCountersGroup : Group
{
    public DailyDivertissmentCountersGroup()
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
