using TrackYourLife.Modules.Trainings.Contracts.Dtos;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;

namespace TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetTrainingStats;

internal static class TrainingStatsTrendBuilder
{
    public static IReadOnlyList<WorkoutAggregatedValueDto> BuildWeeklyDurationTrend(
        IReadOnlyList<OngoingTrainingReadModel> sessions,
        DateOnly windowStart,
        DateOnly windowEnd,
        AggregationType aggregationType
    )
    {
        var durationByDate = sessions
            .Where(w => w.FinishedOnUtc.HasValue)
            .Select(w =>
            {
                var date = DateOnly.FromDateTime(w.FinishedOnUtc!.Value);
                var durationSeconds = (w.FinishedOnUtc.Value - w.StartedOnUtc).TotalSeconds;
                return (date, durationSeconds);
            })
            .GroupBy(x => x.date)
            .ToDictionary(g => g.Key, g => g.Select(x => x.durationSeconds).ToList());

        return BuildWeeklyAggregatedTrend(durationByDate, windowStart, windowEnd, aggregationType);
    }

    public static IReadOnlyList<WorkoutFrequencyDataDto> BuildWeeklyFrequencyTrend(
        IReadOnlyList<OngoingTrainingReadModel> sessions,
        DateOnly windowStart,
        DateOnly windowEnd
    )
    {
        var frequencyByDate = sessions
            .Where(w => w.FinishedOnUtc.HasValue)
            .GroupBy(w => DateOnly.FromDateTime(w.FinishedOnUtc!.Value))
            .ToDictionary(g => g.Key, g => g.Count());

        return BuildWeeklyFrequency(frequencyByDate, windowStart, windowEnd);
    }

    public static IReadOnlyList<WorkoutAggregatedValueDto> BuildWeeklyCaloriesTrend(
        IReadOnlyList<OngoingTrainingReadModel> sessions,
        DateOnly windowStart,
        DateOnly windowEnd,
        AggregationType aggregationType
    )
    {
        var caloriesByDate = sessions
            .Where(w => w.FinishedOnUtc.HasValue && w.CaloriesBurned.HasValue)
            .Select(w =>
            {
                var date = DateOnly.FromDateTime(w.FinishedOnUtc!.Value);
                var calories = (double)w.CaloriesBurned!.Value;
                return (date, calories);
            })
            .GroupBy(x => x.date)
            .ToDictionary(g => g.Key, g => g.Select(x => x.calories).ToList());

        return BuildWeeklyAggregatedTrend(caloriesByDate, windowStart, windowEnd, aggregationType);
    }

    private static List<WorkoutAggregatedValueDto> BuildWeeklyAggregatedTrend(
        Dictionary<DateOnly, List<double>> valuesByDate,
        DateOnly startDate,
        DateOnly endDate,
        AggregationType aggregationType
    )
    {
        var weekGroups = valuesByDate
            .GroupBy(kvp => GetStartOfWeek(kvp.Key))
            .Select(weekGroup =>
            {
                var weekStart = weekGroup.Key;
                var weekEnd = weekStart.AddDays(6);
                var allValues = weekGroup.SelectMany(kvp => kvp.Value).ToList();
                var value = Aggregate(allValues, aggregationType);
                return new WorkoutAggregatedValueDto(weekStart, value, weekStart, weekEnd);
            })
            .OrderBy(w => w.Date)
            .ToList();

        return FillWeeklyAggregatedBuckets(weekGroups, startDate, endDate);
    }

    private static List<WorkoutFrequencyDataDto> BuildWeeklyFrequency(
        Dictionary<DateOnly, int> frequencyByDate,
        DateOnly startDate,
        DateOnly endDate
    )
    {
        var weekGroups = frequencyByDate
            .GroupBy(kvp => GetStartOfWeek(kvp.Key))
            .Select(weekGroup =>
            {
                var weekStart = weekGroup.Key;
                var weekEnd = weekStart.AddDays(6);
                var workoutCount = weekGroup.Sum(kvp => kvp.Value);
                return new WorkoutFrequencyDataDto(weekStart, workoutCount, weekStart, weekEnd);
            })
            .OrderBy(w => w.Date)
            .ToList();

        var filledWeeks = new List<WorkoutFrequencyDataDto>();
        var currentWeekStart = GetStartOfWeek(startDate);
        var endWeekStart = GetStartOfWeek(endDate);

        while (currentWeekStart <= endWeekStart)
        {
            var existingWeek = weekGroups.FirstOrDefault(w => w.Date == currentWeekStart);
            var weekEnd = currentWeekStart.AddDays(6);
            filledWeeks.Add(
                existingWeek
                    ?? new WorkoutFrequencyDataDto(currentWeekStart, 0, currentWeekStart, weekEnd)
            );
            currentWeekStart = currentWeekStart.AddDays(7);
        }

        return filledWeeks;
    }

    private static List<WorkoutAggregatedValueDto> FillWeeklyAggregatedBuckets(
        List<WorkoutAggregatedValueDto> weekGroups,
        DateOnly startDate,
        DateOnly endDate
    )
    {
        var filledWeeks = new List<WorkoutAggregatedValueDto>();
        var currentWeekStart = GetStartOfWeek(startDate);
        var endWeekStart = GetStartOfWeek(endDate);

        while (currentWeekStart <= endWeekStart)
        {
            var existing = weekGroups.FirstOrDefault(w => w.Date == currentWeekStart);
            var weekEnd = currentWeekStart.AddDays(6);
            filledWeeks.Add(
                existing
                    ?? new WorkoutAggregatedValueDto(currentWeekStart, 0, currentWeekStart, weekEnd)
            );
            currentWeekStart = currentWeekStart.AddDays(7);
        }

        return filledWeeks;
    }

    private static double Aggregate(List<double> values, AggregationType aggregationType)
    {
        if (aggregationType == AggregationType.Sum)
        {
            return values.Sum();
        }

        return values.Count > 0 ? values.Average() : 0;
    }

    private static DateOnly GetStartOfWeek(DateOnly date)
    {
        var daysToSubtract = date.DayOfWeek == DayOfWeek.Sunday ? 6 : (int)date.DayOfWeek - 1;
        return date.AddDays(-daysToSubtract);
    }
}
