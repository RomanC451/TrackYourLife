using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Reading.Domain.Features.Books;
using TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Reading.Infrastructure.Data.ReadingSessions;

internal sealed class ReadingSessionsQuery(ReadingReadDbContext context) : IReadingSessionsQuery
{
    public Task<ReadingSessionReadModel?> GetByIdAsync(
        ReadingSessionId id,
        CancellationToken cancellationToken = default
    ) =>
        ProjectSessions(context.ReadingSessions.Where(s => s.Id == id))
            .FirstOrDefaultAsync(cancellationToken);

    public Task<ReadingSessionReadModel?> GetActiveByUserIdAsync(
        UserId userId,
        CancellationToken cancellationToken = default
    ) =>
        ProjectSessions(
                context.ReadingSessions.Where(s => s.UserId == userId && s.FinishedOnUtc == null)
            )
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<IReadOnlyList<ReadingSessionReadModel>> GetFinishedByUserIdAsync(
        UserId userId,
        CancellationToken cancellationToken = default
    ) =>
        await ProjectSessions(
                context
                    .ReadingSessions.Where(s => s.UserId == userId && s.FinishedOnUtc != null)
                    .OrderByDescending(s => s.SessionDate)
                    .ThenByDescending(s => s.FinishedOnUtc)
            )
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<ReadingSessionReadModel>> GetFinishedByBookIdAsync(
        BookId bookId,
        UserId userId,
        CancellationToken cancellationToken = default
    ) =>
        await ProjectSessions(
                context
                    .ReadingSessions.Where(s =>
                        s.BookId == bookId && s.UserId == userId && s.FinishedOnUtc != null
                    )
                    .OrderBy(s => s.SessionDate)
                    .ThenBy(s => s.FinishedOnUtc)
            )
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<ReadingSessionReadModel>> GetFinishedByUserIdAndDateRangeAsync(
        UserId userId,
        DateOnly? fromDate,
        DateOnly? toDate,
        CancellationToken cancellationToken = default
    )
    {
        var query = context.ReadingSessions.Where(s =>
            s.UserId == userId && s.FinishedOnUtc != null
        );

        if (fromDate.HasValue)
        {
            query = query.Where(s => s.SessionDate >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(s => s.SessionDate <= toDate.Value);
        }

        return await ProjectSessions(query.OrderByDescending(s => s.SessionDate))
            .ToListAsync(cancellationToken);
    }

    public async Task<int?> GetMaxEndPageForBookAsync(
        BookId bookId,
        UserId userId,
        CancellationToken cancellationToken = default
    )
    {
        var max = await context
            .ReadingSessions.Where(s =>
                s.BookId == bookId && s.UserId == userId && s.FinishedOnUtc != null
            )
            .MaxAsync(s => s.EndPage, cancellationToken);

        return max;
    }

    private IQueryable<ReadingSessionReadModel> ProjectSessions(
        IQueryable<ReadingSessionReadModel> sessions
    ) =>
        from session in sessions
        join book in context.Books on session.BookId equals book.Id into bookJoin
        from book in bookJoin.DefaultIfEmpty()
        select new ReadingSessionReadModel(
            session.Id,
            session.UserId,
            session.BookId,
            session.SessionDate,
            session.StartPage,
            session.EndPage,
            session.PagesRead,
            session.DurationSeconds,
            session.Notes,
            session.StartedOnUtc,
            session.FinishedOnUtc,
            session.CreatedOnUtc
        )
        {
            BookTitle = book != null ? book.Title : string.Empty,
            BookAuthor = book != null ? book.Author : string.Empty,
        };
}
