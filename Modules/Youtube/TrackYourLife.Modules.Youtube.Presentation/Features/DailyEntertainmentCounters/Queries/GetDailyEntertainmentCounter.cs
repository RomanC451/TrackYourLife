using TrackYourLife.Modules.Youtube.Application.Features.DailyEntertainmentCounters.Queries.GetDailyEntertainmentCounter;
using TrackYourLife.Modules.Youtube.Presentation.Features.DailyEntertainmentCounters.Models;

namespace TrackYourLife.Modules.Youtube.Presentation.Features.DailyEntertainmentCounters.Queries;

internal sealed class GetDailyEntertainmentCounter(ISender sender) : Endpoint<EmptyRequest, IResult>
{
    public override void Configure()
    {
        Get("daily-counter");
        Group<DailyEntertainmentCountersGroup>();
        Description(x =>
            x.Produces<DailyEntertainmentCounterDto>(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status401Unauthorized)
        );
    }

    public override async Task<IResult> ExecuteAsync(EmptyRequest req, CancellationToken ct)
    {
        return await Result
            .Create(new GetDailyEntertainmentCounterQuery())
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync(counter => counter?.ToDto());
    }
}
