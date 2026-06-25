using TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.Reading.Application.Features.ReadingSessions.Commands.UpdateReadingSessionNote;

internal sealed class UpdateReadingSessionNoteCommandHandler(
    IReadingSessionNotesRepository readingSessionNotesRepository,
    IUserIdentifierProvider userIdentifierProvider
) : ICommandHandler<UpdateReadingSessionNoteCommand>
{
    public async Task<Result> Handle(
        UpdateReadingSessionNoteCommand command,
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

        var updateResult = note.Update(command.ChapterTitle, command.Content);

        if (updateResult.IsFailure)
        {
            return updateResult;
        }

        readingSessionNotesRepository.Update(note);

        return Result.Success();
    }
}
