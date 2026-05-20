using TrackYourLife.Modules.Youtube.Application.Features.DailyCategoryWatchCounters.Queries.GetDailyCategoryWatchCounters;
using TrackYourLife.Modules.Youtube.Presentation.Features.DailyCategoryWatchCounters.Models;

namespace TrackYourLife.Modules.Youtube.Presentation.Features.DailyCategoryWatchCounters.Queries;

internal sealed class GetDailyCategoryWatchCounters(ISender sender) : Endpoint<EmptyRequest, IResult>
{
    public override void Configure()
    {
        Get("daily-category-watch-counters");
        Group<DailyCategoryWatchCountersGroup>();
        Description(x =>
            x.Produces<IEnumerable<DailyCategoryWatchCounterDto>>(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status401Unauthorized)
        );
    }

    public override async Task<IResult> ExecuteAsync(EmptyRequest req, CancellationToken ct)
    {
        return await Result
            .Create(new GetDailyCategoryWatchCountersQuery())
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync(rows => rows.Select(r => r.ToDto()));
    }
}
