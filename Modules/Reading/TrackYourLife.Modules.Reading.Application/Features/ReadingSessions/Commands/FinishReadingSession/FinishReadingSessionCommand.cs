using TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;

namespace TrackYourLife.Modules.Reading.Application.Features.ReadingSessions.Commands.FinishReadingSession;

public sealed record FinishReadingSessionCommand(
    ReadingSessionId Id,
    int EndPage,
    DateOnly? SessionDate,
    int? DurationSeconds
) : ICommand<bool>;
