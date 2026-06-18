using TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;

namespace TrackYourLife.Modules.Reading.Application.Features.ReadingSessions.Queries.GetActiveReadingSession;

public sealed record GetActiveReadingSessionQuery : IQuery<ReadingSessionReadModel?>;
