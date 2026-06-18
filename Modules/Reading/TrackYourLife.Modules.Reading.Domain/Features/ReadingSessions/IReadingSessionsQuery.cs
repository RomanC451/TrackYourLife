using TrackYourLife.Modules.Reading.Domain.Features.Books;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;

public interface IReadingSessionsQuery
{
    Task<ReadingSessionReadModel?> GetByIdAsync(
        ReadingSessionId id,
        CancellationToken cancellationToken = default
    );

    Task<ReadingSessionReadModel?> GetActiveByUserIdAsync(
        UserId userId,
        CancellationToken cancellationToken = default
    );

    Task<IReadOnlyList<ReadingSessionReadModel>> GetFinishedByUserIdAsync(
        UserId userId,
        CancellationToken cancellationToken = default
    );

    Task<IReadOnlyList<ReadingSessionReadModel>> GetFinishedByBookIdAsync(
        BookId bookId,
        UserId userId,
        CancellationToken cancellationToken = default
    );

    Task<IReadOnlyList<ReadingSessionReadModel>> GetFinishedByUserIdAndDateRangeAsync(
        UserId userId,
        DateOnly? fromDate,
        DateOnly? toDate,
        CancellationToken cancellationToken = default
    );

    Task<int?> GetMaxEndPageForBookAsync(
        BookId bookId,
        UserId userId,
        CancellationToken cancellationToken = default
    );
}
