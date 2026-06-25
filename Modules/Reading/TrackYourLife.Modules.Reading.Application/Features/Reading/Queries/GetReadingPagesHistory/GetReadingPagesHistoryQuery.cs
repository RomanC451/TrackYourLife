using TrackYourLife.Modules.Reading.Contracts.Dtos;
using TrackYourLife.Modules.Reading.Domain.Features.Reading;

namespace TrackYourLife.Modules.Reading.Application.Features.Reading.Queries.GetReadingPagesHistory;

public sealed record GetReadingPagesHistoryQuery(
    DateOnly? StartDate = null,
    DateOnly? EndDate = null,
    ReadingOverviewType OverviewType = ReadingOverviewType.Weekly
) : IQuery<IReadOnlyList<ReadingPagesDataPointDto>>;
