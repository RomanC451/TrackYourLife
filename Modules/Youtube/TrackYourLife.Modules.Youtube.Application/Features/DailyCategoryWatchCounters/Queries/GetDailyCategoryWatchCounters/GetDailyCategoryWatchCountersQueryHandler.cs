using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Domain.Features.DailyCategoryWatchCounters;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Application.Features.DailyCategoryWatchCounters.Queries.GetDailyCategoryWatchCounters;

internal sealed class GetDailyCategoryWatchCountersQueryHandler(
    IUserIdentifierProvider userIdentifierProvider,
    IDailyCategoryWatchCountersQuery dailyCategoryWatchCountersQuery,
    IDateTimeProvider dateTimeProvider
) : IQueryHandler<GetDailyCategoryWatchCountersQuery, IReadOnlyList<DailyCategoryWatchCounterReadModel>>
{
    public async Task<Result<IReadOnlyList<DailyCategoryWatchCounterReadModel>>> Handle(
        GetDailyCategoryWatchCountersQuery request,
        CancellationToken cancellationToken
    )
    {
        var userId = userIdentifierProvider.UserId;
        var today = DateOnly.FromDateTime(dateTimeProvider.UtcNow);

        var rows = await dailyCategoryWatchCountersQuery.ListByUserIdAndDateAsync(
            userId,
            today,
            cancellationToken
        );

        return Result.Success(rows);
    }
}
