using TrackYourLife.Modules.Reading.Contracts.Dtos;

namespace TrackYourLife.Modules.Reading.Application.Features.Reading.Queries.GetDailyReadingProgress;

public sealed record GetDailyReadingProgressQuery(DateOnly? Date)
    : IQuery<DailyReadingProgressDto>;
