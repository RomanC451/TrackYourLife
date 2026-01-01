using TrackYourLife.Modules.Youtube.Application.Features.DailyDivertissmentCounters.Queries.GetDailyDivertissmentCounter;
using TrackYourLife.Modules.Youtube.Presentation.Features.DailyDivertissmentCounters.Models;

namespace TrackYourLife.Modules.Youtube.Presentation.Features.DailyDivertissmentCounters.Queries;

internal sealed class GetDailyDivertissmentCounter(ISender sender) : Endpoint<EmptyRequest, IResult>
{
    public override void Configure()
    {
        Get("daily-counter");
        Group<DailyDivertissmentCountersGroup>();
        Description(x =>
            x.Produces<DailyDivertissmentCounterDto>(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status401Unauthorized)
        );
    }

    public override async Task<IResult> ExecuteAsync(EmptyRequest req, CancellationToken ct)
    {
        return await Result
            .Create(new GetDailyDivertissmentCounterQuery())
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync(counter => counter?.ToDto());
    }
}
