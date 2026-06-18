using TrackYourLife.Modules.Reading.Domain.Features.Books;
using TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Reading.Application.Features.ReadingSessions.Services;

internal static class BookProgressReconciler
{
    public static async Task<Result> ReconcileBookCurrentPageAsync(
        BookId bookId,
        UserId userId,
        IBooksRepository booksRepository,
        IReadingSessionsRepository readingSessionsRepository,
        CancellationToken cancellationToken,
        int? additionalEndPage = null
    )
    {
        var book = await booksRepository.GetByIdAsync(bookId, cancellationToken);

        if (book is null)
        {
            return Result.Failure(BookErrors.NotFound(bookId));
        }

        if (book.UserId != userId)
        {
            return Result.Failure(BookErrors.NotOwned(bookId));
        }

        var maxEndPage = await readingSessionsRepository.GetMaxFinishedEndPageForBookAsync(
            bookId,
            userId,
            cancellationToken
        );

        if (additionalEndPage.HasValue)
        {
            maxEndPage = maxEndPage.HasValue
                ? Math.Max(maxEndPage.Value, additionalEndPage.Value)
                : additionalEndPage.Value;
        }

        if (maxEndPage is null)
        {
            return Result.Success();
        }

        var updateResult = book.UpdateCurrentPage(maxEndPage.Value);

        if (updateResult.IsFailure)
        {
            return updateResult;
        }

        booksRepository.Update(book);

        return Result.Success();
    }
}
