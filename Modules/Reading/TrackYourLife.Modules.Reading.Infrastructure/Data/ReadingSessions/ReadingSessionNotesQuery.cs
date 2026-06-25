using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Reading.Domain.Features.Books;
using TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Reading.Infrastructure.Data.ReadingSessions;

internal sealed class ReadingSessionNotesQuery(ReadingReadDbContext context) : IReadingSessionNotesQuery
{
    public async Task<IReadOnlyList<ReadingSessionNoteReadModel>> GetByBookIdAsync(
        BookId bookId,
        UserId userId,
        CancellationToken cancellationToken = default
    ) =>
        await ProjectNotes(
                context
                    .ReadingSessionNotes.Where(note => note.BookId == bookId && note.UserId == userId)
                    .OrderByDescending(note => note.CreatedOnUtc)
            )
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<ReadingSessionNoteReadModel>> GetBySessionIdAsync(
        ReadingSessionId sessionId,
        UserId userId,
        CancellationToken cancellationToken = default
    ) =>
        await ProjectNotes(
                context
                    .ReadingSessionNotes.Where(note =>
                        note.ReadingSessionId == sessionId && note.UserId == userId
                    )
                    .OrderByDescending(note => note.CreatedOnUtc)
            )
            .ToListAsync(cancellationToken);

    public async Task<RandomReadingNoteReadModel?> GetRandomByUserIdAsync(
        UserId userId,
        CancellationToken cancellationToken = default
    )
    {
        var query =
            from note in context.ReadingSessionNotes
            join book in context.Books on note.BookId equals book.Id
            where note.UserId == userId
            select new RandomReadingNoteReadModel(
                note.Id,
                note.BookId,
                book.Title,
                note.ChapterTitle,
                note.Content
            );

        var count = await query.CountAsync(cancellationToken);
        if (count == 0)
        {
            return null;
        }

        var skip = Random.Shared.Next(count);
        return await query.Skip(skip).Take(1).FirstOrDefaultAsync(cancellationToken);
    }

    private IQueryable<ReadingSessionNoteReadModel> ProjectNotes(
        IQueryable<ReadingSessionNoteReadModel> notes
    ) =>
        from note in notes
        join session in context.ReadingSessions on note.ReadingSessionId equals session.Id into sessionJoin
        from session in sessionJoin.DefaultIfEmpty()
        select new ReadingSessionNoteReadModel(
            note.Id,
            note.ReadingSessionId,
            note.BookId,
            note.UserId,
            note.ChapterTitle,
            note.Content,
            note.CreatedOnUtc,
            session != null ? session.SessionDate : null
        );
}
