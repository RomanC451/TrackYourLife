using TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.Reading.Application.Features.ReadingSessions.Queries.GetReadingSessionHistory;

internal sealed class GetReadingSessionHistoryQueryHandler(
    IReadingSessionsQuery readingSessionsQuery,
    IUserIdentifierProvider userIdentifierProvider
) : IQueryHandler<GetReadingSessionHistoryQuery, IReadOnlyList<ReadingSessionReadModel>>
{
    public async Task<Result<IReadOnlyList<ReadingSessionReadModel>>> Handle(
        GetReadingSessionHistoryQuery request,
        CancellationToken cancellationToken
    )
    {
        var sessions = await readingSessionsQuery.GetFinishedByUserIdAsync(
            userIdentifierProvider.UserId,
            cancellationToken
        );

        return Result.Success(sessions);
    }
}
