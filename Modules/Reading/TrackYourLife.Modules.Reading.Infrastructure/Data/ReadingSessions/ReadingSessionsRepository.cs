using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Reading.Domain.Features.Books;
using TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Reading.Infrastructure.Data.ReadingSessions;

internal sealed class ReadingSessionsRepository(ReadingWriteDbContext context)
    : GenericRepository<ReadingSession, ReadingSessionId>(context.ReadingSessions),
        IReadingSessionsRepository
{
    public Task<ReadingSession?> GetActiveByUserIdAsync(
        UserId userId,
        CancellationToken cancellationToken = default
    ) =>
        query.FirstOrDefaultAsync(
            s => s.UserId == userId && s.FinishedOnUtc == null,
            cancellationToken
        );

    public async Task<int?> GetMaxFinishedEndPageForBookAsync(
        BookId bookId,
        UserId userId,
        CancellationToken cancellationToken = default
    )
    {
        var finishedSessions = await query
            .Where(s => s.BookId == bookId && s.UserId == userId && s.FinishedOnUtc != null)
            .ToListAsync(cancellationToken);

        if (finishedSessions.Count == 0)
        {
            return null;
        }

        return finishedSessions.Max(s => s.EndPage!.Value);
    }
}
