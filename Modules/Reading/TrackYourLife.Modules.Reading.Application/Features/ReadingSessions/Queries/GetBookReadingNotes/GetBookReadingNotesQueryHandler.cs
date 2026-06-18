using TrackYourLife.Modules.Reading.Application.Features.ReadingSessions.Services;
using TrackYourLife.Modules.Reading.Contracts.Dtos;
using TrackYourLife.Modules.Reading.Domain.Features.Books;
using TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.Reading.Application.Features.ReadingSessions.Queries.GetBookReadingNotes;

internal sealed class GetBookReadingNotesQueryHandler(
    IReadingSessionNotesQuery readingSessionNotesQuery,
    IBooksQuery booksQuery,
    IUserIdentifierProvider userIdentifierProvider
) : IQueryHandler<GetBookReadingNotesQuery, IReadOnlyList<BookChapterNotesGroupDto>>
{
    public async Task<Result<IReadOnlyList<BookChapterNotesGroupDto>>> Handle(
        GetBookReadingNotesQuery request,
        CancellationToken cancellationToken
    )
    {
        var book = await booksQuery.GetByIdAsync(request.BookId, cancellationToken);

        if (book is null)
        {
            return Result.Failure<IReadOnlyList<BookChapterNotesGroupDto>>(
                BookErrors.NotFound(request.BookId)
            );
        }

        if (book.UserId != userIdentifierProvider.UserId)
        {
            return Result.Failure<IReadOnlyList<BookChapterNotesGroupDto>>(
                BookErrors.NotOwned(request.BookId)
            );
        }

        var notes = await readingSessionNotesQuery.GetByBookIdAsync(
            request.BookId,
            userIdentifierProvider.UserId,
            cancellationToken
        );

        return Result.Success(ReadingSessionNotesGrouper.GroupByChapter(notes));
    }
}
