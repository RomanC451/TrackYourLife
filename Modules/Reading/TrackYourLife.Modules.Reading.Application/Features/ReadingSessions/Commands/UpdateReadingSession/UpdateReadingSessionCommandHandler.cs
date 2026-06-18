using TrackYourLife.Modules.Reading.Application.Features.ReadingSessions.Services;
using TrackYourLife.Modules.Reading.Domain.Features.Books;
using TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.Reading.Application.Features.ReadingSessions.Commands.UpdateReadingSession;

internal sealed class UpdateReadingSessionCommandHandler(
    IReadingSessionsRepository readingSessionsRepository,
    IBooksRepository booksRepository,
    IUserIdentifierProvider userIdentifierProvider
) : ICommandHandler<UpdateReadingSessionCommand, bool>
{
    public async Task<Result<bool>> Handle(
        UpdateReadingSessionCommand command,
        CancellationToken cancellationToken
    )
    {
        var session = await readingSessionsRepository.GetByIdAsync(command.Id, cancellationToken);

        if (session is null)
        {
            return Result.Failure<bool>(ReadingSessionErrors.NotFound(command.Id));
        }

        if (session.UserId != userIdentifierProvider.UserId)
        {
            return Result.Failure<bool>(ReadingSessionErrors.NotOwned(command.Id));
        }

        var book = await booksRepository.GetByIdAsync(session.BookId, cancellationToken);

        if (book is null)
        {
            return Result.Failure<bool>(BookErrors.NotFound(session.BookId));
        }

        var updateResult = session.UpdateFinished(
            command.EndPage,
            book.TotalPages,
            command.SessionDate,
            command.Notes,
            command.DurationSeconds
        );

        if (updateResult.IsFailure)
        {
            return Result.Failure<bool>(updateResult.Error);
        }

        readingSessionsRepository.Update(session);

        var reconcileResult = await BookProgressReconciler.ReconcileBookCurrentPageAsync(
            session.BookId,
            userIdentifierProvider.UserId,
            booksRepository,
            readingSessionsRepository,
            cancellationToken,
            command.EndPage
        );

        if (reconcileResult.IsFailure)
        {
            return Result.Failure<bool>(reconcileResult.Error);
        }

        var updatedBook = await booksRepository.GetByIdAsync(session.BookId, cancellationToken);

        return Result.Success(updatedBook?.SuggestMarkAsFinished() ?? false);
    }
}
