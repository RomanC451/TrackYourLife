using TrackYourLife.Modules.Reading.Application.Features.ReadingSessions.Services;
using TrackYourLife.Modules.Reading.Domain.Features.Books;
using TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.Reading.Application.Features.ReadingSessions.Commands.DeleteReadingSession;

internal sealed class DeleteReadingSessionCommandHandler(
    IReadingSessionsRepository readingSessionsRepository,
    IBooksRepository booksRepository,
    IUserIdentifierProvider userIdentifierProvider
) : ICommandHandler<DeleteReadingSessionCommand>
{
    public async Task<Result> Handle(
        DeleteReadingSessionCommand command,
        CancellationToken cancellationToken
    )
    {
        var session = await readingSessionsRepository.GetByIdAsync(command.Id, cancellationToken);

        if (session is null)
        {
            return Result.Failure(ReadingSessionErrors.NotFound(command.Id));
        }

        if (session.UserId != userIdentifierProvider.UserId)
        {
            return Result.Failure(ReadingSessionErrors.NotOwned(command.Id));
        }

        if (session.IsActive)
        {
            return Result.Failure(ReadingSessionErrors.SessionNotFinished);
        }

        var bookId = session.BookId;
        readingSessionsRepository.Remove(session);

        var book = await booksRepository.GetByIdAsync(bookId, cancellationToken);

        if (book is null)
        {
            return Result.Success();
        }

        await BookProgressReconciler.ReconcileBookCurrentPageAsync(
            bookId,
            userIdentifierProvider.UserId,
            booksRepository,
            readingSessionsRepository,
            cancellationToken
        );

        return Result.Success();
    }
}
