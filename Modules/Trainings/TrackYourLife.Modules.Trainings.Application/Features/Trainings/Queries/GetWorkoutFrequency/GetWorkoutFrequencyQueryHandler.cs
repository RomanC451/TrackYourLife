using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Contracts.Dtos;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetWorkoutFrequency;

public class GetWorkoutFrequencyQueryHandler(
    IUserIdentifierProvider userIdentifierProvider,
    IOngoingTrainingsQuery ongoingTrainingsQuery
) : IQueryHandler<GetWorkoutFrequencyQuery, IReadOnlyList<WorkoutFrequencyDataDto>>
{
    public async Task<Result<IReadOnlyList<WorkoutFrequencyDataDto>>> Handle(
        GetWorkoutFrequencyQuery request,
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
                return Result.Success<IReadOnlyList<WorkoutFrequencyDataDto>>([]);
            }
            frequencyStartDate = workoutsWithDate.Min();
            frequencyEndDate = workoutsWithDate.Max();
        }

        var workoutFrequencyByDate = completedWorkouts
            .Where(w => w.FinishedOnUtc.HasValue)
            .GroupBy(w => DateOnly.FromDateTime(w.FinishedOnUtc!.Value))
            .ToDictionary(g => g.Key, g => g.Count());

        var workoutFrequency = request.OverviewType switch
        {
            OverviewType.Daily => GenerateDailyFrequency(
                workoutFrequencyByDate,
                frequencyStartDate,
                frequencyEndDate
            ),
            OverviewType.Weekly => GenerateWeeklyFrequency(
                workoutFrequencyByDate,
                frequencyStartDate,
                frequencyEndDate
            ),
            OverviewType.Monthly => GenerateMonthlyFrequency(
                workoutFrequencyByDate,
                frequencyStartDate,
                frequencyEndDate
            ),
            _ => GenerateDailyFrequency(
                workoutFrequencyByDate,
                frequencyStartDate,
                frequencyEndDate
            ),
        };

        return Result.Success<IReadOnlyList<WorkoutFrequencyDataDto>>(workoutFrequency);
    }

    private static List<WorkoutFrequencyDataDto> GenerateDailyFrequency(
        Dictionary<DateOnly, int> workoutFrequencyByDate,
        DateOnly startDate,
        DateOnly endDate
    )
    {
        var workoutFrequency = new List<WorkoutFrequencyDataDto>();
        var currentDate = startDate;
        while (currentDate <= endDate)
        {
            var workoutCount = workoutFrequencyByDate.GetValueOrDefault(currentDate, 0);
            workoutFrequency.Add(
                new WorkoutFrequencyDataDto(currentDate, workoutCount, currentDate, currentDate)
            );
            currentDate = currentDate.AddDays(1);
        }

        return workoutFrequency;
    }

    private static List<WorkoutFrequencyDataDto> GenerateWeeklyFrequency(
        Dictionary<DateOnly, int> workoutFrequencyByDate,
        DateOnly startDate,
        DateOnly endDate
    )
    {
        var weekGroups = workoutFrequencyByDate
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
            if (existingWeek != null)
            {
                filledWeeks.Add(existingWeek);
            }
            else
            {
                var weekEnd = currentWeekStart.AddDays(6);
                filledWeeks.Add(
                    new WorkoutFrequencyDataDto(currentWeekStart, 0, currentWeekStart, weekEnd)
                );
            }

            currentWeekStart = currentWeekStart.AddDays(7);
        }

        return filledWeeks;
    }

    private static List<WorkoutFrequencyDataDto> GenerateMonthlyFrequency(
        Dictionary<DateOnly, int> workoutFrequencyByDate,
        DateOnly startDate,
        DateOnly endDate
    )
    {
        var monthGroups = workoutFrequencyByDate
            .GroupBy(kvp => new { kvp.Key.Year, kvp.Key.Month })
            .Select(monthGroup =>
            {
                var firstDayOfMonth = new DateOnly(monthGroup.Key.Year, monthGroup.Key.Month, 1);
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
                var workoutCount = monthGroup.Sum(kvp => kvp.Value);
                return new WorkoutFrequencyDataDto(
                    firstDayOfMonth,
                    workoutCount,
                    firstDayOfMonth,
                    lastDayOfMonth
                );
            })
            .OrderBy(w => w.Date)
            .ToList();

        var filledMonths = new List<WorkoutFrequencyDataDto>();
        var currentMonth = new DateOnly(startDate.Year, startDate.Month, 1);
        var endMonth = new DateOnly(endDate.Year, endDate.Month, 1);

        while (currentMonth <= endMonth)
        {
            var existingMonth = monthGroups.FirstOrDefault(w => w.Date == currentMonth);
            if (existingMonth != null)
            {
                filledMonths.Add(existingMonth);
            }
            else
            {
                var lastDayOfMonth = currentMonth.AddMonths(1).AddDays(-1);
                filledMonths.Add(
                    new WorkoutFrequencyDataDto(currentMonth, 0, currentMonth, lastDayOfMonth)
                );
            }

            currentMonth = currentMonth.AddMonths(1);
        }

        return filledMonths;
    }

    private static DateOnly GetStartOfWeek(DateOnly date)
    {
        var dayOfWeek = date.DayOfWeek;
        var daysToSubtract = dayOfWeek == DayOfWeek.Sunday ? 6 : (int)dayOfWeek - 1;
        return date.AddDays(-daysToSubtract);
    }
}
