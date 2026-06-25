using TrackYourLife.Modules.Reading.Contracts.Dtos;
using TrackYourLife.Modules.Reading.Domain.Features.Reading;
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

    public IReadOnlyList<ReadingPagesDataPointDto> CalculatePagesHistory(
        IReadOnlyList<ReadingSessionReadModel> finishedSessions,
        DateOnly startDate,
        DateOnly endDate,
        ReadingOverviewType overviewType
    )
    {
        var pagesByDate = finishedSessions
            .Where(s => s.SessionDate.HasValue)
            .GroupBy(s => s.SessionDate!.Value)
            .ToDictionary(g => g.Key, g => g.Sum(s => s.PagesRead ?? 0));

        return overviewType switch
        {
            ReadingOverviewType.Daily => GenerateDaily(pagesByDate, startDate, endDate),
            ReadingOverviewType.Weekly => GenerateWeekly(pagesByDate, startDate, endDate),
            ReadingOverviewType.Monthly => GenerateMonthly(pagesByDate, startDate, endDate),
            _ => GenerateDaily(pagesByDate, startDate, endDate),
        };
    }

    private static List<ReadingPagesDataPointDto> GenerateDaily(
        Dictionary<DateOnly, int> pagesByDate,
        DateOnly startDate,
        DateOnly endDate
    )
    {
        var result = new List<ReadingPagesDataPointDto>();
        var currentDate = startDate;

        while (currentDate <= endDate)
        {
            var pages = pagesByDate.GetValueOrDefault(currentDate);
            result.Add(new ReadingPagesDataPointDto(currentDate, pages, currentDate, currentDate));
            currentDate = currentDate.AddDays(1);
        }

        return result;
    }

    private static List<ReadingPagesDataPointDto> GenerateWeekly(
        Dictionary<DateOnly, int> pagesByDate,
        DateOnly startDate,
        DateOnly endDate
    )
    {
        var weekGroups = pagesByDate
            .GroupBy(kvp => GetStartOfWeek(kvp.Key))
            .Select(weekGroup =>
            {
                var weekStart = weekGroup.Key;
                var weekEnd = weekStart.AddDays(6);
                var pages = weekGroup.Sum(kvp => kvp.Value);
                return new ReadingPagesDataPointDto(weekStart, pages, weekStart, weekEnd);
            })
            .OrderBy(w => w.Date)
            .ToList();

        var filledWeeks = new List<ReadingPagesDataPointDto>();
        var currentWeekStart = GetStartOfWeek(startDate);
        var endWeekStart = GetStartOfWeek(endDate);

        while (currentWeekStart <= endWeekStart)
        {
            var existing = weekGroups.FirstOrDefault(w => w.Date == currentWeekStart);
            var weekEnd = currentWeekStart.AddDays(6);
            filledWeeks.Add(
                existing
                    ?? new ReadingPagesDataPointDto(currentWeekStart, 0, currentWeekStart, weekEnd)
            );
            currentWeekStart = currentWeekStart.AddDays(7);
        }

        return filledWeeks;
    }

    private static List<ReadingPagesDataPointDto> GenerateMonthly(
        Dictionary<DateOnly, int> pagesByDate,
        DateOnly startDate,
        DateOnly endDate
    )
    {
        var monthGroups = pagesByDate
            .GroupBy(kvp => new { kvp.Key.Year, kvp.Key.Month })
            .Select(monthGroup =>
            {
                var firstDay = new DateOnly(monthGroup.Key.Year, monthGroup.Key.Month, 1);
                var lastDay = firstDay.AddMonths(1).AddDays(-1);
                var pages = monthGroup.Sum(kvp => kvp.Value);
                return new ReadingPagesDataPointDto(firstDay, pages, firstDay, lastDay);
            })
            .OrderBy(w => w.Date)
            .ToList();

        var filledMonths = new List<ReadingPagesDataPointDto>();
        var currentMonth = new DateOnly(startDate.Year, startDate.Month, 1);
        var endMonth = new DateOnly(endDate.Year, endDate.Month, 1);

        while (currentMonth <= endMonth)
        {
            var existing = monthGroups.FirstOrDefault(w => w.Date == currentMonth);
            var lastDay = currentMonth.AddMonths(1).AddDays(-1);
            filledMonths.Add(
                existing ?? new ReadingPagesDataPointDto(currentMonth, 0, currentMonth, lastDay)
            );
            currentMonth = currentMonth.AddMonths(1);
        }

        return filledMonths;
    }

    private static DateOnly GetStartOfWeek(DateOnly date)
    {
        var daysToSubtract = date.DayOfWeek == DayOfWeek.Sunday ? 6 : (int)date.DayOfWeek - 1;
        return date.AddDays(-daysToSubtract);
    }
}
