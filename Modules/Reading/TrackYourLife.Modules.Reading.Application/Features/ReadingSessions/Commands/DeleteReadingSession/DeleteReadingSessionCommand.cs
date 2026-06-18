using TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;

namespace TrackYourLife.Modules.Reading.Application.Features.ReadingSessions.Commands.DeleteReadingSession;

public sealed record DeleteReadingSessionCommand(ReadingSessionId Id) : ICommand;
