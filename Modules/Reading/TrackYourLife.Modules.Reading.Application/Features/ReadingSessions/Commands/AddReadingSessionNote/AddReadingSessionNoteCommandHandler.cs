using TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.Reading.Application.Features.ReadingSessions.Commands.AddReadingSessionNote;

internal sealed class AddReadingSessionNoteCommandHandler(
    IReadingSessionsRepository readingSessionsRepository,
    IReadingSessionNotesRepository readingSessionNotesRepository,
    IUserIdentifierProvider userIdentifierProvider
) : ICommandHandler<AddReadingSessionNoteCommand, ReadingSessionNoteId>
{
    public async Task<Result<ReadingSessionNoteId>> Handle(
        AddReadingSessionNoteCommand command,
        CancellationToken cancellationToken
    )
    {
        var session = await readingSessionsRepository.GetByIdAsync(
            command.SessionId,
            cancellationToken
        );

        if (session is null)
        {
            return Result.Failure<ReadingSessionNoteId>(
                ReadingSessionErrors.NotFound(command.SessionId)
            );
        }

        if (session.UserId != userIdentifierProvider.UserId)
        {
            return Result.Failure<ReadingSessionNoteId>(
                ReadingSessionErrors.NotOwned(command.SessionId)
            );
        }

        if (!session.IsActive)
        {
            return Result.Failure<ReadingSessionNoteId>(
                ReadingSessionErrors.SessionAlreadyFinished
            );
        }

        var noteResult = ReadingSessionNote.Create(
            ReadingSessionNoteId.NewId(),
            session.Id,
            session.BookId,
            session.UserId,
            command.ChapterTitle,
            command.Content,
            DateTime.UtcNow
        );

        if (noteResult.IsFailure)
        {
            return Result.Failure<ReadingSessionNoteId>(noteResult.Error);
        }

        await readingSessionNotesRepository.AddAsync(noteResult.Value, cancellationToken);

        return Result.Success(noteResult.Value.Id);
    }
}
