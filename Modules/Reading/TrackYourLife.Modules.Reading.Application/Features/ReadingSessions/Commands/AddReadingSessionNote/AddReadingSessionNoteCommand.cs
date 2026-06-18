using TrackYourLife.Modules.Reading.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;

namespace TrackYourLife.Modules.Reading.Application.Features.ReadingSessions.Commands.AddReadingSessionNote;

public sealed record AddReadingSessionNoteCommand(
    ReadingSessionId SessionId,
    string ChapterTitle,
    string Content
) : ICommand<ReadingSessionNoteId>;
