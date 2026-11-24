using TrackYourLife.Modules.Reading.Application.Features.ReadingSessions.Services;
using TrackYourLife.Modules.Reading.Contracts.Dtos;
using TrackYourLife.Modules.Reading.Domain.Features.Books;
using TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Reading.Application.Features.ReadingSessions.Commands.LogReadingProgress;

internal sealed class LogReadingProgressCommandHandler(
    IBooksRepository booksRepository,
    IBooksQuery booksQuery,
    IReadingSessionsRepository readingSessionsRepository,
    IReadingSessionsQuery readingSessionsQuery,
    IUserIdentifierProvider userIdentifierProvider
) : ICommandHandler<LogReadingProgressCommand, LogReadingProgressDto>
{
    public async Task<Result<LogReadingProgressDto>> Handle(
        LogReadingProgressCommand command,
        CancellationToken cancellationToken
    )
    {
        var userId = userIdentifierProvider.UserId;

        var bookId = await ResolveLastOngoingBookIdAsync(userId, cancellationToken);

        if (bookId is null)
        {
            return Result.Failure<LogReadingProgressDto>(BookErrors.NoOngoingBook);
        }

        var book = await booksRepository.GetByIdAsync(bookId, cancellationToken);

        if (book is null)
        {
            return Result.Failure<LogReadingProgressDto>(BookErrors.NotFound(bookId));
        }

        if (book.UserId != userId)
        {
            return Result.Failure<LogReadingProgressDto>(BookErrors.NotOwned(bookId));
        }

        var startPage = book.CurrentPage;

        if (command.EndPage <= startPage)
        {
            return Result.Failure<LogReadingProgressDto>(BookErrors.EndPageNotAfterCurrent);
        }

        var now = DateTime.UtcNow;
        var sessionDate = command.SessionDate ?? DateOnly.FromDateTime(now);

        var sessionResult = ReadingSession.CreateFinished(
            ReadingSessionId.NewId(),
            userId,
            book.Id,
            startPage,
            command.EndPage,
            book.TotalPages,
            sessionDate,
            now,
            now
        );

        if (sessionResult.IsFailure)
        {
            return Result.Failure<LogReadingProgressDto>(sessionResult.Error);
        }

        var session = sessionResult.Value;

        await readingSessionsRepository.AddAsync(session, cancellationToken);

        var reconcileResult = await BookProgressReconciler.ReconcileBookCurrentPageAsync(
            book.Id,
            userId,
            booksRepository,
            readingSessionsRepository,
            cancellationToken,
            command.EndPage
        );

        if (reconcileResult.IsFailure)
        {
            return Result.Failure<LogReadingProgressDto>(reconcileResult.Error);
        }

        var updatedBook = await booksRepository.GetByIdAsync(book.Id, cancellationToken);

        return Result.Success(
            new LogReadingProgressDto(
                session.Id,
                book.Id,
                book.Title,
                startPage,
                command.EndPage,
                session.PagesRead ?? (command.EndPage - startPage),
                updatedBook?.SuggestMarkAsFinished() ?? false
            )
        );
    }

    private async Task<BookId?> ResolveLastOngoingBookIdAsync(
        UserId userId,
        CancellationToken cancellationToken
    )
    {
        var ongoingBooks = await booksQuery.GetByUserIdAsync(
            userId,
            BookStatus.Ongoing,
            BookSortField.StartingDate,
            sortDescending: true,
            cancellationToken
        );

        if (ongoingBooks.Count == 0)
        {
            return null;
        }

        var ongoingIds = ongoingBooks.Select(book => book.Id).ToHashSet();

        var finishedSessions = await readingSessionsQuery.GetFinishedByUserIdAsync(
            userId,
            cancellationToken
        );

        var lastReadOngoing = finishedSessions
            .Select(session => session.BookId)
            .FirstOrDefault(bookId => ongoingIds.Contains(bookId));

        if (lastReadOngoing is not null)
        {
            return lastReadOngoing;
        }

        return ongoingBooks
            .OrderByDescending(book => book.ModifiedOnUtc ?? book.CreatedOnUtc)
            .ThenByDescending(book => book.StartingDate)
            .Select(book => book.Id)
            .First();
    }
}
