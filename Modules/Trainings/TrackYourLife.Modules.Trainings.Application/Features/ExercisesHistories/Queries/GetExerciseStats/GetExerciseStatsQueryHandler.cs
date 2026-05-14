using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Contracts.Dtos;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.Features.ExercisesHistories.Queries.GetExerciseStats;

public sealed class GetExerciseStatsQueryHandler(
    IUserIdentifierProvider userIdentifierProvider,
    IExercisesHistoriesQuery exercisesHistoriesQuery,
    IExercisesQuery exercisesQuery
) : IQueryHandler<GetExerciseStatsQuery, ExerciseStatsDto>
{
    public async Task<Result<ExerciseStatsDto>> Handle(
        GetExerciseStatsQuery request,
        CancellationToken cancellationToken
    )
    {
        var exercise = await exercisesQuery.GetByIdAsync(request.ExerciseId, cancellationToken);
        if (exercise is null)
        {
            return Result.Failure<ExerciseStatsDto>(ExercisesErrors.NotFoundById(request.ExerciseId));
        }

        var userId = userIdentifierProvider.UserId;
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var allHistories = (
            await exercisesHistoriesQuery.GetCompletedByUserIdAsync(userId, cancellationToken)
        )
            .Where(h => h.ExerciseId == request.ExerciseId)
            .ToList();

        var (windowStart, windowEnd) = ResolveWindow(
            today,
            request.Range,
            request.StartDate,
            request.EndDate,
            allHistories
        );

        var inWindow = allHistories
            .Where(h =>
            {
                var d = DateOnly.FromDateTime(h.CreatedOnUtc);
                return d >= windowStart && d <= windowEnd;
            })
            .ToList();

        var completedInWindow = inWindow
            .Where(h => h.Status == ExerciseStatus.Completed)
            .OrderBy(h => h.CreatedOnUtc)
            .ToList();

        var skippedCount = inWindow.Count(h => h.Status == ExerciseStatus.Skipped);

        var isSupportedExerciseType =
            completedInWindow.Count == 0
            || completedInWindow.All(h => IsWeightRepsHistory(h));

        if (!isSupportedExerciseType)
        {
            return Result.Success(
                new ExerciseStatsDto(
                    exercise.Id,
                    exercise.Name,
                    request.Range,
                    windowStart,
                    windowEnd,
                    request.ChartMetric,
                    false,
                    false,
                    new ExerciseStatsSummaryDto(0, 0, 0, 0, skippedCount),
                    [],
                    BuildConsistencyTrend(inWindow, windowStart, windowEnd)
                )
            );
        }

        var totalVolume = completedInWindow.Sum(h => CalculateVolume(h.NewExerciseSets));
        var completedCount = completedInWindow.Count;
        var averageVolume = completedCount > 0 ? totalVolume / completedCount : 0.0;

        var hasEnoughData = completedCount >= 2;
        var improvementPercent = ComputeFirstVsLastVolumeImprovementPercent(completedInWindow);

        var improvementTrend = BuildImprovementTrend(
            completedInWindow,
            request.ChartMetric,
            windowStart,
            windowEnd
        );

        var consistencyTrend = BuildConsistencyTrend(inWindow, windowStart, windowEnd);

        var dto = new ExerciseStatsDto(
            exercise.Id,
            exercise.Name,
            request.Range,
            windowStart,
            windowEnd,
            request.ChartMetric,
            true,
            hasEnoughData,
            new ExerciseStatsSummaryDto(
                improvementPercent,
                averageVolume,
                totalVolume,
                completedCount,
                skippedCount
            ),
            improvementTrend,
            consistencyTrend
        );

        return Result.Success(dto);
    }

    private static (DateOnly Start, DateOnly End) ResolveWindow(
        DateOnly today,
        ExerciseStatsRange range,
        DateOnly? customStart,
        DateOnly? customEnd,
        IReadOnlyList<ExerciseHistoryReadModel> exerciseHistories
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
            var dates = exerciseHistories
                .Select(h => DateOnly.FromDateTime(h.CreatedOnUtc))
                .ToList();
            start = dates.Count > 0 ? dates.Min() : end;
        }
        else
        {
            var rangeStart = ResolveRangeStartDate(today, range);
            start = rangeStart ?? exerciseHistories
                .Select(h => DateOnly.FromDateTime(h.CreatedOnUtc))
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

    private static double ComputeFirstVsLastVolumeImprovementPercent(
        IReadOnlyList<ExerciseHistoryReadModel> completedOrdered
    )
    {
        if (completedOrdered.Count < 2)
        {
            return 0;
        }

        var vFirst = CalculateVolume(completedOrdered[0].NewExerciseSets);
        var vLast = CalculateVolume(completedOrdered[^1].NewExerciseSets);

        if (vFirst > 1e-9)
        {
            return (vLast - vFirst) / vFirst * 100.0;
        }

        if (vFirst <= 1e-9 && vLast > 1e-9)
        {
            return 100.0;
        }

        return 0;
    }

    private static IReadOnlyList<ExerciseImprovementTrendPointDto> BuildImprovementTrend(
        IReadOnlyList<ExerciseHistoryReadModel> completedInWindow,
        ExerciseStatsChartMetric chartMetric,
        DateOnly windowStart,
        DateOnly windowEnd
    )
    {
        var byDay = completedInWindow
            .GroupBy(h => DateOnly.FromDateTime(h.CreatedOnUtc))
            .OrderBy(g => g.Key)
            .ToList();

        var points = new List<ExerciseImprovementTrendPointDto>();
        foreach (var group in byDay)
        {
            if (group.Key < windowStart || group.Key > windowEnd)
            {
                continue;
            }

            var value = AggregateMetricForDay(group.ToList(), chartMetric);
            points.Add(new ExerciseImprovementTrendPointDto(group.Key, value));
        }

        return points;
    }

    private static double AggregateMetricForDay(
        IReadOnlyList<ExerciseHistoryReadModel> dayHistories,
        ExerciseStatsChartMetric metric
    )
    {
        var sets = dayHistories.SelectMany(h => h.NewExerciseSets).ToList();
        if (sets.Count == 0)
        {
            return 0;
        }

        return metric switch
        {
            ExerciseStatsChartMetric.Volume => dayHistories.Sum(h => CalculateVolume(h.NewExerciseSets)),
            ExerciseStatsChartMetric.TotalWeight => sets.Sum(GetWeightOrZero),
            ExerciseStatsChartMetric.MaxWeight => sets.Max(GetWeightOrZero),
            ExerciseStatsChartMetric.MinWeight => sets.Where(s => GetWeightOrZero(s) > 0).Select(GetWeightOrZero).DefaultIfEmpty(0).Min(),
            ExerciseStatsChartMetric.TotalReps => sets.Sum(GetRepsOrZero),
            ExerciseStatsChartMetric.MaxReps => sets.Max(GetRepsOrZero),
            ExerciseStatsChartMetric.MinReps => sets.Where(s => GetRepsOrZero(s) > 0).Select(GetRepsOrZero).DefaultIfEmpty(0).Min(),
            _ => dayHistories.Sum(h => CalculateVolume(h.NewExerciseSets)),
        };
    }

    private static IReadOnlyList<ExerciseConsistencyPointDto> BuildConsistencyTrend(
        IReadOnlyList<ExerciseHistoryReadModel> inWindow,
        DateOnly windowStart,
        DateOnly windowEnd
    )
    {
        var byWeek = inWindow
            .GroupBy(h => GetStartOfWeek(DateOnly.FromDateTime(h.CreatedOnUtc)))
            .OrderBy(g => g.Key)
            .ToList();

        var list = new List<ExerciseConsistencyPointDto>();
        foreach (var group in byWeek)
        {
            if (group.Key > windowEnd)
            {
                continue;
            }

            var weekEnd = group.Key.AddDays(6);
            if (weekEnd < windowStart)
            {
                continue;
            }

            var completed = group.Count(h => h.Status == ExerciseStatus.Completed);
            var skipped = group.Count(h => h.Status == ExerciseStatus.Skipped);
            list.Add(new ExerciseConsistencyPointDto(group.Key, completed, skipped));
        }

        return list;
    }

    private static DateOnly GetStartOfWeek(DateOnly date)
    {
        var dayOfWeek = (int)date.DayOfWeek;
        var daysToSubtract = dayOfWeek == 0 ? 6 : dayOfWeek - 1;
        return date.AddDays(-daysToSubtract);
    }

    private static bool IsWeightRepsHistory(ExerciseHistoryReadModel history)
    {
        return history.NewExerciseSets.Count > 0 && history.NewExerciseSets.All(IsWeightRepsSet);
    }

    private static bool IsWeightRepsSet(ExerciseSet set)
    {
        return set.Count2.HasValue
            && !string.IsNullOrWhiteSpace(set.Unit1)
            && !string.IsNullOrWhiteSpace(set.Unit2)
            && TryGetWeightAndReps(set).HasValue;
    }

    private static (float Weight, float Reps)? TryGetWeightAndReps(ExerciseSet set)
    {
        if (!set.Count2.HasValue)
        {
            return null;
        }

        var u1 = set.Unit1.Trim();
        var u2 = set.Unit2!.Trim();
        var w1 = LooksLikeWeight(u1);
        var w2 = LooksLikeWeight(u2);
        var r1 = LooksLikeReps(u1);
        var r2 = LooksLikeReps(u2);

        if (w1 && r2)
        {
            return (set.Count1, set.Count2.Value);
        }

        if (r1 && w2)
        {
            return (set.Count2.Value, set.Count1);
        }

        return null;
    }

    private static bool LooksLikeWeight(string unit)
    {
        var u = unit.ToLowerInvariant();
        return u.Contains("kg", StringComparison.Ordinal)
            || u.Contains("lb", StringComparison.Ordinal);
    }

    private static bool LooksLikeReps(string unit)
    {
        return unit.ToLowerInvariant().Contains("rep", StringComparison.Ordinal);
    }

    private static float GetWeightOrZero(ExerciseSet set)
    {
        return TryGetWeightAndReps(set)?.Weight ?? 0f;
    }

    private static float GetRepsOrZero(ExerciseSet set)
    {
        return TryGetWeightAndReps(set)?.Reps ?? 0f;
    }

    private static double CalculateVolume(IReadOnlyList<ExerciseSet> sets)
    {
        double sum = 0;
        foreach (var set in sets)
        {
            var pair = TryGetWeightAndReps(set);
            if (pair is { } p)
            {
                sum += p.Weight * p.Reps;
            }
        }

        return sum;
    }
}
