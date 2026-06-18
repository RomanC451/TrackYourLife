using TrackYourLife.Modules.Reading.Domain.Features.Books;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;

public interface IReadingSessionsRepository
{
    Task<ReadingSession?> GetByIdAsync(
        ReadingSessionId id,
        CancellationToken cancellationToken = default
    );

    Task<ReadingSession?> GetActiveByUserIdAsync(
        UserId userId,
        CancellationToken cancellationToken = default
    );

    Task AddAsync(ReadingSession session, CancellationToken cancellationToken = default);

    void Update(ReadingSession session);

    void Remove(ReadingSession session);

    Task<int?> GetMaxFinishedEndPageForBookAsync(
        BookId bookId,
        UserId userId,
        CancellationToken cancellationToken = default
    );
}
