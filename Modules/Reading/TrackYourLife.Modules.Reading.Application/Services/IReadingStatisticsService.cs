using TrackYourLife.Modules.Reading.Contracts.Dtos;
using TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;

namespace TrackYourLife.Modules.Reading.Application.Services;

public interface IReadingStatisticsService
{
    DailyReadingProgressDto CalculateDailyProgress(
        IReadOnlyList<ReadingSessionReadModel> finishedSessions,
        DateOnly date,
        int? targetPages
    );

    ReadingStreakDto CalculateStreak(
        IReadOnlyList<ReadingSessionReadModel> finishedSessions,
        Func<DateOnly, int?> getTargetForDate,
        DateOnly today
    );
}
