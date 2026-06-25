using TrackYourLife.Modules.Reading.Domain.Features.Books;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;

public interface IReadingSessionNotesQuery
{
    Task<IReadOnlyList<ReadingSessionNoteReadModel>> GetByBookIdAsync(
        BookId bookId,
        UserId userId,
        CancellationToken cancellationToken = default
    );

    Task<IReadOnlyList<ReadingSessionNoteReadModel>> GetBySessionIdAsync(
        ReadingSessionId sessionId,
        UserId userId,
        CancellationToken cancellationToken = default
    );

    Task<IReadOnlyList<ReadingSessionNoteReadModel>> GetRecentByUserIdAsync(
        UserId userId,
        int take,
        CancellationToken cancellationToken = default
    );
}
