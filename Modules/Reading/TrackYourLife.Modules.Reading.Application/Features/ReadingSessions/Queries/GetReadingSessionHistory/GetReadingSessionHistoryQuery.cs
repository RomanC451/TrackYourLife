using TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;

namespace TrackYourLife.Modules.Reading.Application.Features.ReadingSessions.Queries.GetReadingSessionHistory;

public sealed record GetReadingSessionHistoryQuery : IQuery<IReadOnlyList<ReadingSessionReadModel>>;
