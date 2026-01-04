using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Domain.Features.DailyEntertainmentCounters;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Application.Features.DailyEntertainmentCounters.Queries.GetDailyEntertainmentCounter;

internal sealed class GetDailyEntertainmentCounterQueryHandler(
    IUserIdentifierProvider userIdentifierProvider,
    IDailyEntertainmentCountersRepository dailyEntertainmentCountersRepository,
    IDateTimeProvider dateTimeProvider
) : IQueryHandler<GetDailyEntertainmentCounterQuery, DailyEntertainmentCounterReadModel?>
{
    public async Task<Result<DailyEntertainmentCounterReadModel?>> Handle(
        GetDailyEntertainmentCounterQuery request,
        CancellationToken cancellationToken
    )
    {
        var userId = userIdentifierProvider.UserId;
        var today = DateOnly.FromDateTime(dateTimeProvider.UtcNow);

        var counter = await dailyEntertainmentCountersRepository.GetByUserIdAndDateAsync(
            userId,
            today,
            cancellationToken
        );

        if (counter is null)
        {
            return Result.Success<DailyEntertainmentCounterReadModel?>(null);
        }

        var readModel = new DailyEntertainmentCounterReadModel(
            counter.Id,
            counter.UserId,
            counter.Date,
            counter.VideosWatchedCount
        );

        return Result.Success<DailyEntertainmentCounterReadModel?>(readModel);
    }
}
