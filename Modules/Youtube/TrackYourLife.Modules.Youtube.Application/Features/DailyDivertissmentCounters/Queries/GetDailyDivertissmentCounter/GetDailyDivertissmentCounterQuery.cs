using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Domain.Features.DailyDivertissmentCounters;

namespace TrackYourLife.Modules.Youtube.Application.Features.DailyDivertissmentCounters.Queries.GetDailyDivertissmentCounter;

public sealed record GetDailyDivertissmentCounterQuery()
    : IQuery<DailyDivertissmentCounterReadModel?>;
