using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Contracts.Dtos;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetWorkoutDurationHistory;

public class GetWorkoutDurationHistoryQueryHandler(
    IUserIdentifierProvider userIdentifierProvider,
    IOngoingTrainingsQuery ongoingTrainingsQuery
) : IQueryHandler<GetWorkoutDurationHistoryQuery, IReadOnlyList<WorkoutAggregatedValueDto>>
{
    public async Task<Result<IReadOnlyList<WorkoutAggregatedValueDto>>> Handle(
        GetWorkoutDurationHistoryQuery request,
        CancellationToken cancellationToken
    )
    {
        var userId = userIdentifierProvider.UserId;

        IEnumerable<OngoingTrainingReadModel> completedWorkouts;
        DateOnly frequencyStartDate;
        DateOnly frequencyEndDate;

        if (request.StartDate.HasValue && request.EndDate.HasValue)
        {
            completedWorkouts = await ongoingTrainingsQuery.GetCompletedByUserIdAndDateRangeAsync(
                userId,
                request.StartDate.Value,
                request.EndDate.Value,
                cancellationToken
            );
            frequencyStartDate = request.StartDate.Value;
            frequencyEndDate = request.EndDate.Value;
        }
        else
        {
            completedWorkouts = await ongoingTrainingsQuery.GetCompletedByUserIdAsync(
                userId,
                cancellationToken
            );
            var workoutsWithDate = completedWorkouts
                .Where(w => w.FinishedOnUtc.HasValue)
                .Select(w => DateOnly.FromDateTime(w.FinishedOnUtc!.Value))
                .ToList();
            if (workoutsWithDate.Count == 0)
            {
                return Result.Success<IReadOnlyList<WorkoutAggregatedValueDto>>([]);
            }
            frequencyStartDate = workoutsWithDate.Min();
            frequencyEndDate = workoutsWithDate.Max();
        }

        var durationByDate = completedWorkouts
            .Where(w => w.FinishedOnUtc.HasValue)
            .Select(w =>
            {
                var date = DateOnly.FromDateTime(w.FinishedOnUtc!.Value);
                var durationSeconds = (w.FinishedOnUtc.Value - w.StartedOnUtc).TotalSeconds;
                return (date, durationSeconds);
            })
            .GroupBy(x => x.date)
            .ToDictionary(g => g.Key, g => g.Select(x => x.durationSeconds).ToList());

        var aggregated = request.OverviewType switch
        {
            OverviewType.Daily => GenerateDaily(
                durationByDate,
                frequencyStartDate,
                frequencyEndDate,
                request.AggregationType
            ),
            OverviewType.Weekly => GenerateWeekly(
                durationByDate,
                frequencyStartDate,
                frequencyEndDate,
                request.AggregationType
            ),
            OverviewType.Monthly => GenerateMonthly(
                durationByDate,
                frequencyStartDate,
                frequencyEndDate,
                request.AggregationType
            ),
            _ => GenerateDaily(
                durationByDate,
                frequencyStartDate,
                frequencyEndDate,
                request.AggregationType
            ),
        };

        return Result.Success<IReadOnlyList<WorkoutAggregatedValueDto>>(aggregated);
    }

    private static List<WorkoutAggregatedValueDto> GenerateDaily(
        Dictionary<DateOnly, List<double>> durationByDate,
        DateOnly startDate,
        DateOnly endDate,
        AggregationType aggregationType
    )
    {
        var result = new List<WorkoutAggregatedValueDto>();
        var currentDate = startDate;
        while (currentDate <= endDate)
        {
            var values = durationByDate.GetValueOrDefault(currentDate, []);
            var value = Aggregate(values, aggregationType);
            result.Add(new WorkoutAggregatedValueDto(currentDate, value, currentDate, currentDate));
            currentDate = currentDate.AddDays(1);
        }

        return result;
    }

    private static List<WorkoutAggregatedValueDto> GenerateWeekly(
        Dictionary<DateOnly, List<double>> durationByDate,
        DateOnly startDate,
        DateOnly endDate,
        AggregationType aggregationType
    )
    {
        var weekGroups = durationByDate
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

    private static List<WorkoutAggregatedValueDto> GenerateMonthly(
        Dictionary<DateOnly, List<double>> durationByDate,
        DateOnly startDate,
        DateOnly endDate,
        AggregationType aggregationType
    )
    {
        var monthGroups = durationByDate
            .GroupBy(kvp => new { kvp.Key.Year, kvp.Key.Month })
            .Select(monthGroup =>
            {
                var firstDay = new DateOnly(monthGroup.Key.Year, monthGroup.Key.Month, 1);
                var lastDay = firstDay.AddMonths(1).AddDays(-1);
                var allValues = monthGroup.SelectMany(kvp => kvp.Value).ToList();
                var value = Aggregate(allValues, aggregationType);
                return new WorkoutAggregatedValueDto(firstDay, value, firstDay, lastDay);
            })
            .OrderBy(w => w.Date)
            .ToList();

        var filledMonths = new List<WorkoutAggregatedValueDto>();
        var currentMonth = new DateOnly(startDate.Year, startDate.Month, 1);
        var endMonth = new DateOnly(endDate.Year, endDate.Month, 1);

        while (currentMonth <= endMonth)
        {
            var existing = monthGroups.FirstOrDefault(w => w.Date == currentMonth);
            var lastDay = currentMonth.AddMonths(1).AddDays(-1);
            filledMonths.Add(
                existing ?? new WorkoutAggregatedValueDto(currentMonth, 0, currentMonth, lastDay)
            );
            currentMonth = currentMonth.AddMonths(1);
        }

        return filledMonths;
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
