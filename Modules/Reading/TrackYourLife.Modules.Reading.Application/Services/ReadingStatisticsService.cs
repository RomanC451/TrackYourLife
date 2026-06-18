using TrackYourLife.Modules.Reading.Contracts.Dtos;
using TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;

namespace TrackYourLife.Modules.Reading.Application.Services;

internal sealed class ReadingStatisticsService : IReadingStatisticsService
{
    public DailyReadingProgressDto CalculateDailyProgress(
        IReadOnlyList<ReadingSessionReadModel> finishedSessions,
        DateOnly date,
        int? targetPages
    )
    {
        var pagesReadToday = finishedSessions
            .Where(s => s.SessionDate == date)
            .Sum(s => s.PagesRead ?? 0);

        if (targetPages is null or <= 0)
        {
            return new DailyReadingProgressDto(
                TargetPages: null,
                PagesReadToday: pagesReadToday,
                Remaining: 0,
                TargetMet: false,
                HasTarget: false
            );
        }

        var remaining = Math.Max(0, targetPages.Value - pagesReadToday);

        return new DailyReadingProgressDto(
            TargetPages: targetPages,
            PagesReadToday: pagesReadToday,
            Remaining: remaining,
            TargetMet: pagesReadToday >= targetPages.Value,
            HasTarget: true
        );
    }

    public ReadingStreakDto CalculateStreak(
        IReadOnlyList<ReadingSessionReadModel> finishedSessions,
        Func<DateOnly, int?> getTargetForDate,
        DateOnly today
    )
    {
        var pagesByDate = finishedSessions
            .Where(s => s.SessionDate.HasValue)
            .GroupBy(s => s.SessionDate!.Value)
            .ToDictionary(g => g.Key, g => g.Sum(s => s.PagesRead ?? 0));

        var successfulDays = pagesByDate
            .Select(kvp =>
            {
                var target = getTargetForDate(kvp.Key);
                var met = target is > 0 && kvp.Value >= target.Value;
                return new ReadingStreakDayDto(kvp.Key, met, kvp.Value);
            })
            .Where(d => d.TargetMet)
            .OrderByDescending(d => d.Date)
            .ToList();

        var todayTarget = getTargetForDate(today);
        var todayPages = pagesByDate.GetValueOrDefault(today);
        var todayTargetMet = todayTarget is > 0 && todayPages >= todayTarget.Value;

        var yesterday = today.AddDays(-1);
        var currentStreak = 0;
        var cursor = todayTargetMet ? today : yesterday;

        if (
            (todayTargetMet || pagesByDate.ContainsKey(yesterday))
            && getTargetForDate(cursor) is > 0
        )
        {
            while (true)
            {
                var target = getTargetForDate(cursor);
                if (target is null or <= 0)
                {
                    break;
                }

                var pages = pagesByDate.GetValueOrDefault(cursor);
                if (pages < target.Value)
                {
                    break;
                }

                currentStreak++;
                cursor = cursor.AddDays(-1);
            }
        }

        var longestStreak = CalculateLongestStreak(pagesByDate, getTargetForDate);

        return new ReadingStreakDto(
            currentStreak,
            longestStreak,
            todayTargetMet,
            successfulDays
        );
    }

    private static int CalculateLongestStreak(
        Dictionary<DateOnly, int> pagesByDate,
        Func<DateOnly, int?> getTargetForDate
    )
    {
        if (pagesByDate.Count == 0)
        {
            return 0;
        }

        var dates = pagesByDate.Keys.OrderBy(d => d).ToList();
        var longest = 0;
        var current = 0;
        DateOnly? previousDate = null;

        foreach (var date in dates)
        {
            var target = getTargetForDate(date);
            var met = target is > 0 && pagesByDate[date] >= target.Value;

            if (!met)
            {
                current = 0;
                previousDate = date;
                continue;
            }

            if (previousDate is not null && date == previousDate.Value.AddDays(1))
            {
                current++;
            }
            else
            {
                current = 1;
            }

            longest = Math.Max(longest, current);
            previousDate = date;
        }

        return longest;
    }
}
