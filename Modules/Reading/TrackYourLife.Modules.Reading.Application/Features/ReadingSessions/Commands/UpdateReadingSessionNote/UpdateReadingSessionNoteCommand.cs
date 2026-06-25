using TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;

namespace TrackYourLife.Modules.Reading.Application.Features.ReadingSessions.Commands.UpdateReadingSessionNote;

public sealed record UpdateReadingSessionNoteCommand(
    ReadingSessionId SessionId,
    ReadingSessionNoteId NoteId,
    string ChapterTitle,
    string Content
) : ICommand;
