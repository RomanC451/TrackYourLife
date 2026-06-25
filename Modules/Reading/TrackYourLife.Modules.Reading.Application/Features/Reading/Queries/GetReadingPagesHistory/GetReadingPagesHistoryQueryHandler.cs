using TrackYourLife.Modules.Reading.Application.Services;
using TrackYourLife.Modules.Reading.Contracts.Dtos;
using TrackYourLife.Modules.Reading.Domain.Features.Reading;
using TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.Reading.Application.Features.Reading.Queries.GetReadingPagesHistory;

internal sealed class GetReadingPagesHistoryQueryHandler(
    IReadingSessionsQuery readingSessionsQuery,
    IReadingStatisticsService readingStatisticsService,
    IUserIdentifierProvider userIdentifierProvider
) : IQueryHandler<GetReadingPagesHistoryQuery, IReadOnlyList<ReadingPagesDataPointDto>>
{
    public async Task<Result<IReadOnlyList<ReadingPagesDataPointDto>>> Handle(
        GetReadingPagesHistoryQuery request,
        CancellationToken cancellationToken
    )
    {
        var userId = userIdentifierProvider.UserId;
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var (startDate, endDate) = ResolveDateRange(request, today);

        var sessions = await readingSessionsQuery.GetFinishedByUserIdAndDateRangeAsync(
            userId,
            startDate,
            endDate,
            cancellationToken
        );

        var history = readingStatisticsService.CalculatePagesHistory(
            sessions,
            startDate,
            endDate,
            request.OverviewType
        );

        return Result.Success(history);
    }

    internal static (DateOnly StartDate, DateOnly EndDate) ResolveDateRange(
        GetReadingPagesHistoryQuery request,
        DateOnly today
    )
    {
        if (request.StartDate.HasValue && request.EndDate.HasValue)
        {
            return (request.StartDate.Value, request.EndDate.Value);
        }

        return request.OverviewType switch
        {
            ReadingOverviewType.Daily => (today.AddDays(-29), today),
            ReadingOverviewType.Weekly => (GetStartOfWeek(today.AddDays(-7 * 11)), today),
            ReadingOverviewType.Monthly => (
                new DateOnly(today.Year, today.Month, 1).AddMonths(-11),
                today
            ),
            _ => (GetStartOfWeek(today.AddDays(-7 * 11)), today),
        };
    }

    private static DateOnly GetStartOfWeek(DateOnly date)
    {
        var daysToSubtract = date.DayOfWeek == DayOfWeek.Sunday ? 6 : (int)date.DayOfWeek - 1;
        return date.AddDays(-daysToSubtract);
    }
}
