using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Domain.Features.DailyEntertainmentCounters;

namespace TrackYourLife.Modules.Youtube.Application.Features.DailyEntertainmentCounters.Queries.GetDailyEntertainmentCounter;

public sealed record GetDailyEntertainmentCounterQuery()
    : IQuery<DailyEntertainmentCounterReadModel?>;
