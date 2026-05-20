using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Domain.Features.DailyCategoryWatchCounters;

namespace TrackYourLife.Modules.Youtube.Application.Features.DailyCategoryWatchCounters.Queries.GetDailyCategoryWatchCounters;

public sealed record GetDailyCategoryWatchCountersQuery()
    : IQuery<IReadOnlyList<DailyCategoryWatchCounterReadModel>>;
