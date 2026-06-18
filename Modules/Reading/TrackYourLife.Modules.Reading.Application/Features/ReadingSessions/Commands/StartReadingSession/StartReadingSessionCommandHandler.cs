using TrackYourLife.Modules.Reading.Domain.Features.Books;
using TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.Reading.Application.Features.ReadingSessions.Commands.StartReadingSession;

internal sealed class StartReadingSessionCommandHandler(
    IReadingSessionsRepository readingSessionsRepository,
    IBooksRepository booksRepository,
    IUserIdentifierProvider userIdentifierProvider
) : ICommandHandler<StartReadingSessionCommand, ReadingSessionId>
{
    public async Task<Result<ReadingSessionId>> Handle(
        StartReadingSessionCommand command,
        CancellationToken cancellationToken
    )
    {
        var existingActive = await readingSessionsRepository.GetActiveByUserIdAsync(
            userIdentifierProvider.UserId,
            cancellationToken
        );

        if (existingActive is not null)
        {
            return Result.Failure<ReadingSessionId>(ReadingSessionErrors.ActiveSessionExists);
        }

        var book = await booksRepository.GetByIdAsync(command.BookId, cancellationToken);

        if (book is null)
        {
            return Result.Failure<ReadingSessionId>(BookErrors.NotFound(command.BookId));
        }

        if (book.UserId != userIdentifierProvider.UserId)
        {
            return Result.Failure<ReadingSessionId>(BookErrors.NotOwned(command.BookId));
        }

        var startedOnUtc = DateTime.UtcNow;
        var beginReadingResult = book.BeginReading(DateOnly.FromDateTime(startedOnUtc));

        if (beginReadingResult.IsFailure)
        {
            return Result.Failure<ReadingSessionId>(beginReadingResult.Error);
        }

        var result = ReadingSession.Start(
            ReadingSessionId.NewId(),
            userIdentifierProvider.UserId,
            command.BookId,
            book.CurrentPage,
            startedOnUtc
        );

        if (result.IsFailure)
        {
            return Result.Failure<ReadingSessionId>(result.Error);
        }

        booksRepository.Update(book);
        await readingSessionsRepository.AddAsync(result.Value, cancellationToken);

        return Result.Success(result.Value.Id);
    }
}
