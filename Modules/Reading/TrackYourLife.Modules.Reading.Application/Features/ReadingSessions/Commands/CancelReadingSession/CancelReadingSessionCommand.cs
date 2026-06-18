using TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;

namespace TrackYourLife.Modules.Reading.Application.Features.ReadingSessions.Commands.CancelReadingSession;

public sealed record CancelReadingSessionCommand(ReadingSessionId Id) : ICommand;
