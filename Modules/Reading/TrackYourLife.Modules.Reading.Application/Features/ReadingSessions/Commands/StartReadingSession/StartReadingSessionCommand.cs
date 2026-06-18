using TrackYourLife.Modules.Reading.Domain.Features.Books;
using TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;

namespace TrackYourLife.Modules.Reading.Application.Features.ReadingSessions.Commands.StartReadingSession;

public sealed record StartReadingSessionCommand(BookId BookId) : ICommand<ReadingSessionId>;
