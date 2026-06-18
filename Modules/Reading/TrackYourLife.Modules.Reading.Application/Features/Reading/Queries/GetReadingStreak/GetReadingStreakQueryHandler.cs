using TrackYourLife.Modules.Reading.Application.Abstraction;
using TrackYourLife.Modules.Reading.Application.Services;
using TrackYourLife.Modules.Reading.Contracts.Dtos;
using TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.Reading.Application.Features.Reading.Queries.GetReadingStreak;

internal sealed class GetReadingStreakQueryHandler(
    IReadingSessionsQuery readingSessionsQuery,
    IReadingGoalProvider readingGoalProvider,
    IReadingStatisticsService readingStatisticsService,
    IUserIdentifierProvider userIdentifierProvider
) : IQueryHandler<GetReadingStreakQuery, ReadingStreakDto>
{
    public async Task<Result<ReadingStreakDto>> Handle(
        GetReadingStreakQuery request,
        CancellationToken cancellationToken
    )
    {
        var userId = userIdentifierProvider.UserId;
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var sessions = await readingSessionsQuery.GetFinishedByUserIdAsync(
            userId,
            cancellationToken
        );

        var targetCache = new Dictionary<DateOnly, int?>();
        var dates = sessions
            .Where(s => s.SessionDate.HasValue)
            .Select(s => s.SessionDate!.Value)
            .Distinct()
            .ToList();

        dates.Add(today);
        dates.Add(today.AddDays(-1));

        foreach (var date in dates.Distinct())
        {
            targetCache[date] = await readingGoalProvider.GetTargetPagesForDateAsync(
                userId,
                date,
                cancellationToken
            );
        }

        int? GetTarget(DateOnly date) => targetCache.GetValueOrDefault(date);

        var streak = readingStatisticsService.CalculateStreak(
            sessions,
            GetTarget,
            today
        );

        return Result.Success(streak);
    }
}
