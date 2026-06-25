using TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.Reading.Application.Features.ReadingSessions.Commands.DeleteReadingSessionNote;

internal sealed class DeleteReadingSessionNoteCommandHandler(
    IReadingSessionNotesRepository readingSessionNotesRepository,
    IUserIdentifierProvider userIdentifierProvider
) : ICommandHandler<DeleteReadingSessionNoteCommand>
{
    public async Task<Result> Handle(
        DeleteReadingSessionNoteCommand command,
        CancellationToken cancellationToken
    )
    {
        var note = await readingSessionNotesRepository.GetByIdAsync(command.NoteId, cancellationToken);

        if (note is null)
        {
            return Result.Failure(ReadingSessionNoteErrors.NotFound(command.NoteId));
        }

        if (note.UserId != userIdentifierProvider.UserId)
        {
            return Result.Failure(ReadingSessionNoteErrors.NotOwned(command.NoteId));
        }

        if (note.ReadingSessionId != command.SessionId)
        {
            return Result.Failure(ReadingSessionNoteErrors.SessionMismatch(command.NoteId));
        }

        readingSessionNotesRepository.Remove(note);

        return Result.Success();
    }
}
