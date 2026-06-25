using TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;

namespace TrackYourLife.Modules.Reading.Application.Features.ReadingSessions.Commands.UpdateReadingSession;

public sealed record UpdateReadingSessionCommand(
    ReadingSessionId Id,
    int EndPage,
    DateOnly SessionDate,
    int? DurationSeconds
) : ICommand<bool>;
