using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Domain.Features.DailyDivertissmentCounters;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Application.Features.DailyDivertissmentCounters.Queries.GetDailyDivertissmentCounter;

internal sealed class GetDailyDivertissmentCounterQueryHandler(
    IUserIdentifierProvider userIdentifierProvider,
    IDailyDivertissmentCountersRepository dailyDivertissmentCountersRepository,
    IDateTimeProvider dateTimeProvider
) : IQueryHandler<GetDailyDivertissmentCounterQuery, DailyDivertissmentCounterReadModel?>
{
    public async Task<Result<DailyDivertissmentCounterReadModel?>> Handle(
        GetDailyDivertissmentCounterQuery request,
        CancellationToken cancellationToken
    )
    {
        var userId = userIdentifierProvider.UserId;
        var today = DateOnly.FromDateTime(dateTimeProvider.UtcNow);

        var counter = await dailyDivertissmentCountersRepository.GetByUserIdAndDateAsync(
            userId,
            today,
            cancellationToken
        );

        if (counter is null)
        {
            return Result.Success<DailyDivertissmentCounterReadModel?>(null);
        }

        var readModel = new DailyDivertissmentCounterReadModel(
            counter.Id,
            counter.UserId,
            counter.Date,
            counter.VideosWatchedCount
        );

        return Result.Success<DailyDivertissmentCounterReadModel?>(readModel);
    }
}
