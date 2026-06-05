using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Contracts.Dtos;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetTrainingStats;

public sealed class GetTrainingStatsQueryHandler(
    IUserIdentifierProvider userIdentifierProvider,
    ITrainingsQuery trainingsQuery,
    IOngoingTrainingsQuery ongoingTrainingsQuery,
    IExercisesHistoriesQuery exercisesHistoriesQuery
) : IQueryHandler<GetTrainingStatsQuery, TrainingStatsDto>
{
    private const int RecentSessionsLimit = 5;

    public async Task<Result<TrainingStatsDto>> Handle(
        GetTrainingStatsQuery request,
        CancellationToken cancellationToken
    )
    {
        var userId = userIdentifierProvider.UserId;

        var training = await trainingsQuery.GetByIdAsync(request.TrainingId, cancellationToken);
        if (training is null)
        {
            return Result.Failure<TrainingStatsDto>(
                TrainingsErrors.NotFoundById(request.TrainingId)
            );
        }

        if (training.UserId != userId)
        {
            return Result.Failure<TrainingStatsDto>(TrainingsErrors.NotOwned(request.TrainingId));
        }

        var allCompletedSessions = (
            await ongoingTrainingsQuery.GetCompletedByUserIdAndTrainingIdAsync(
                userId,
                request.TrainingId,
                cancellationToken
            )
        )
            .Where(w => w.FinishedOnUtc.HasValue)
            .ToList();

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var (windowStart, windowEnd) = ResolveWindow(
            today,
            request.Range,
            request.StartDate,
            request.EndDate,
            allCompletedSessions
        );

        var sessionsInWindow = allCompletedSessions
            .Where(w =>
            {
                var finishedDate = DateOnly.FromDateTime(w.FinishedOnUtc!.Value);
                return finishedDate >= windowStart && finishedDate <= windowEnd;
            })
            .ToList();

        var sessionIds = sessionsInWindow.Select(s => s.Id).ToHashSet();
        var allUserExerciseHistories = await exercisesHistoriesQuery.GetByUserIdAsync(
            userId,
            cancellationToken
        );
        var historiesByOngoingTrainingId = allUserExerciseHistories
            .Where(eh => sessionIds.Contains(eh.OngoingTrainingId))
            .GroupBy(eh => eh.OngoingTrainingId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var hasSkippedBySessionId = sessionIds.ToDictionary(
            id => id,
            id =>
            {
                if (!historiesByOngoingTrainingId.TryGetValue(id, out var histories))
                {
                    return false;
                }

                return histories.Any(eh => eh.Status == ExerciseStatus.Skipped);
            }
        );

        var summary = BuildSummary(
            sessionsInWindow,
            hasSkippedBySessionId,
            windowStart,
            windowEnd
        );

        var durationTrend = TrainingStatsTrendBuilder.BuildWeeklyDurationTrend(
            sessionsInWindow,
            windowStart,
            windowEnd,
            request.ChartAggregationType
        );
        var frequencyTrend = TrainingStatsTrendBuilder.BuildWeeklyFrequencyTrend(
            sessionsInWindow,
            windowStart,
            windowEnd
        );
        var caloriesTrend = TrainingStatsTrendBuilder.BuildWeeklyCaloriesTrend(
            sessionsInWindow,
            windowStart,
            windowEnd,
            request.ChartAggregationType
        );

        var recentSessions = MapRecentSessions(
            sessionsInWindow,
            historiesByOngoingTrainingId,
            training
        );

        var dto = new TrainingStatsDto(
            training.Id,
            training.Name,
            training.Difficulty,
            training.MuscleGroups,
            training.TrainingExercises.Count,
            training.Duration * 60,
            request.Range,
            request.ChartAggregationType,
            summary,
            durationTrend,
            frequencyTrend,
            caloriesTrend,
            recentSessions
        );

        return Result.Success(dto);
    }

    private static TrainingStatsSummaryDto BuildSummary(
        IReadOnlyList<OngoingTrainingReadModel> sessions,
        IReadOnlyDictionary<OngoingTrainingId, bool> hasSkippedBySessionId,
        DateOnly windowStart,
        DateOnly windowEnd
    )
    {
        var sessionsCompleted = sessions.Count;
        var fullyCompleted = sessions.Count(s => !hasSkippedBySessionId[s.Id]);
        var withSkipped = sessions.Count(s => hasSkippedBySessionId[s.Id]);
        var completionRate =
            sessionsCompleted > 0 ? (double)fullyCompleted / sessionsCompleted * 100 : 0;

        var durations = sessions
            .Select(s => (s.FinishedOnUtc!.Value - s.StartedOnUtc).TotalSeconds)
            .ToList();
        var averageDuration = durations.Count > 0 ? durations.Average() : 0;
        var totalDuration = durations.Sum();

        var caloriesValues = sessions
            .Where(s => s.CaloriesBurned.HasValue)
            .Select(s => s.CaloriesBurned!.Value)
            .ToList();
        int? averageCalories =
            caloriesValues.Count > 0 ? (int)Math.Round(caloriesValues.Average()) : null;
        int? totalCalories = caloriesValues.Count > 0 ? caloriesValues.Sum() : null;

        DateTime? lastPerformed = sessions.Count > 0
            ? sessions.Max(s => s.FinishedOnUtc!.Value)
            : null;

        return new TrainingStatsSummaryDto(
            sessionsCompleted,
            fullyCompleted,
            withSkipped,
            completionRate,
            averageDuration,
            totalDuration,
            averageCalories,
            totalCalories,
            lastPerformed,
            windowStart,
            windowEnd
        );
    }

    private static IReadOnlyList<WorkoutHistoryDto> MapRecentSessions(
        IReadOnlyList<OngoingTrainingReadModel> sessions,
        IReadOnlyDictionary<OngoingTrainingId, List<ExerciseHistoryReadModel>> historiesByOngoingId,
        TrainingReadModel training
    )
    {
        var totalExercises = training.TrainingExercises.Count;

        return sessions
            .OrderByDescending(s => s.FinishedOnUtc)
            .Take(RecentSessionsLimit)
            .Select(w =>
            {
                var finishedOnUtc = w.FinishedOnUtc!.Value;
                var histories = historiesByOngoingId.GetValueOrDefault(w.Id);
                var completedExercises =
                    histories == null
                        ? 0
                        : histories.Count(eh => eh.Status == ExerciseStatus.Completed);

                return new WorkoutHistoryDto(
                    w.Id,
                    training.Id,
                    training.Name,
                    w.StartedOnUtc,
                    finishedOnUtc,
                    (long)(finishedOnUtc - w.StartedOnUtc).TotalSeconds,
                    w.CaloriesBurned,
                    completedExercises,
                    totalExercises
                );
            })
            .ToList();
    }

    private static (DateOnly Start, DateOnly End) ResolveWindow(
        DateOnly today,
        ExerciseStatsRange range,
        DateOnly? customStart,
        DateOnly? customEnd,
        IReadOnlyList<OngoingTrainingReadModel> completedSessions
    )
    {
        if (customStart.HasValue && customEnd.HasValue)
        {
            return (customStart.Value, customEnd.Value);
        }

        var end = today;
        DateOnly start;
        if (range == ExerciseStatsRange.All)
        {
            var dates = completedSessions
                .Select(s => DateOnly.FromDateTime(s.FinishedOnUtc!.Value))
                .ToList();
            start = dates.Count > 0 ? dates.Min() : end;
        }
        else
        {
            var rangeStart = ResolveRangeStartDate(today, range);
            start =
                rangeStart
                ?? completedSessions
                    .Select(s => DateOnly.FromDateTime(s.FinishedOnUtc!.Value))
                    .DefaultIfEmpty(end)
                    .Min();
        }

        if (start > end)
        {
            start = end;
        }

        return (start, end);
    }

    private static DateOnly? ResolveRangeStartDate(DateOnly today, ExerciseStatsRange range)
    {
        return range switch
        {
            ExerciseStatsRange.FourWeeks => today.AddDays(-27),
            ExerciseStatsRange.TwelveWeeks => today.AddDays(-83),
            ExerciseStatsRange.SixMonths => today.AddMonths(-6).AddDays(1),
            ExerciseStatsRange.All => null,
            _ => null,
        };
    }
}
