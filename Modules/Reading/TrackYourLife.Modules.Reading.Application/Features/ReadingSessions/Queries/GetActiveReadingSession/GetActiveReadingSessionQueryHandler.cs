using TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.Reading.Application.Features.ReadingSessions.Queries.GetActiveReadingSession;

internal sealed class GetActiveReadingSessionQueryHandler(
    IReadingSessionsQuery readingSessionsQuery,
    IUserIdentifierProvider userIdentifierProvider
) : IQueryHandler<GetActiveReadingSessionQuery, ReadingSessionReadModel?>
{
    public async Task<Result<ReadingSessionReadModel?>> Handle(
        GetActiveReadingSessionQuery request,
        CancellationToken cancellationToken
    )
    {
        var session = await readingSessionsQuery.GetActiveByUserIdAsync(
            userIdentifierProvider.UserId,
            cancellationToken
        );

        return Result.Success(session);
    }
}
