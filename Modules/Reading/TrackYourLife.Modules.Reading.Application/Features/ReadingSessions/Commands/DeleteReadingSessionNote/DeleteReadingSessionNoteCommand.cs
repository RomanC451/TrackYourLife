using TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;

namespace TrackYourLife.Modules.Reading.Application.Features.ReadingSessions.Commands.DeleteReadingSessionNote;

public sealed record DeleteReadingSessionNoteCommand(
    ReadingSessionId SessionId,
    ReadingSessionNoteId NoteId
) : ICommand;
