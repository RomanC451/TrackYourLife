using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Contracts.Dtos;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Contracts.Common;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.Features.ExercisesHistories.Queries.GetExercisePerformance;

public class GetExercisePerformanceQueryHandler(
    IUserIdentifierProvider userIdentifierProvider,
    IExercisesHistoriesQuery exercisesHistoriesQuery,
    IExercisesQuery exercisesQuery
) : IQueryHandler<GetExercisePerformanceQuery, PagedList<ExercisePerformanceDto>>
{
    public async Task<Result<PagedList<ExercisePerformanceDto>>> Handle(
        GetExercisePerformanceQuery request,
        CancellationToken cancellationToken
    )
    {
        var userId = userIdentifierProvider.UserId;

        // Get exercise histories
        IEnumerable<ExerciseHistoryReadModel> exerciseHistories;
        if (request.StartDate.HasValue || request.EndDate.HasValue)
        {
            var startDate = request.StartDate.HasValue
                ? DateTime.SpecifyKind(request.StartDate.Value, DateTimeKind.Utc)
                : DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc);
            var endDate = request.EndDate.HasValue
                ? DateTime.SpecifyKind(request.EndDate.Value, DateTimeKind.Utc)
                : DateTime.SpecifyKind(DateTime.MaxValue, DateTimeKind.Utc);
            exerciseHistories = await exercisesHistoriesQuery.GetByUserIdAndDateRangeAsync(
                userId,
                startDate,
                endDate,
                cancellationToken
            );
        }
        else
        {
            exerciseHistories = await exercisesHistoriesQuery.GetByUserIdAsync(
                userId,
                cancellationToken
            );
        }

        // Filter by exercise ID if provided
        if (request.ExerciseId != null && request.ExerciseId != ExerciseId.Empty)
        {
            exerciseHistories = exerciseHistories.Where(eh => eh.ExerciseId == request.ExerciseId);
        }

        // Get all unique exercise IDs
        var exerciseIds = exerciseHistories.Select(eh => eh.ExerciseId).Distinct().ToList();

        // Get exercise details
        var exercises = await exercisesQuery.GetEnumerableWithinIdsCollectionAsync(
            exerciseIds,
            cancellationToken
        );
        var exerciseDict = exercises.ToDictionary(e => e.Id, e => e.Name);

        // Group by exercise and calculate improvement based on selected method
        var performanceData = exerciseHistories
            .Where(eh => eh.Status == ExerciseStatus.Completed) // Only completed exercises
            .GroupBy(eh => eh.ExerciseId)
            .Select(group =>
            {
                var exerciseId = group.Key;
                var exerciseName = exerciseDict.GetValueOrDefault(exerciseId, "Unknown");

                // Order by date (oldest first) for proper comparison
                var orderedHistories = group.OrderBy(eh => eh.CreatedOnUtc).ToList();

                if (orderedHistories.Count < 2)
                {
                    // Need at least 2 workouts to calculate improvement
                    return new ExercisePerformanceDto(
                        ExerciseId: exerciseId,
                        ExerciseName: exerciseName,
                        ImprovementPercentage: 0.0
                    );
                }

                double improvementPercentage = request.CalculationMethod switch
                {
                    PerformanceCalculationMethod.Sequential => CalculateSequentialImprovement(
                        orderedHistories
                    ),
                    PerformanceCalculationMethod.FirstVsLast => CalculateFirstVsLastImprovement(
                        orderedHistories
                    ),
                    _ => 0.0,
                };

                return new ExercisePerformanceDto(
                    ExerciseId: exerciseId,
                    ExerciseName: exerciseName,
                    ImprovementPercentage: (float)improvementPercentage
                );
            })
            .OrderBy(e => e.ExerciseName)
            .ToList();

        var pagedListResult = PagedList<ExercisePerformanceDto>.Create(
            performanceData,
            request.Page,
            request.PageSize
        );

        if (pagedListResult.IsFailure)
        {
            return Result.Failure<PagedList<ExercisePerformanceDto>>(pagedListResult.Error);
        }

        return Result.Success(pagedListResult.Value);
    }

    /// <summary>
    /// Calculates the total volume from exercise sets.
    /// Volume = count1 × count2 (if count2 is available), otherwise just count1.
    /// </summary>
    private static double CalculateVolume(IReadOnlyList<Domain.Features.Exercises.ExerciseSet> sets)
    {
        return sets.Sum(set =>
        {
            var volume = set.Count2.HasValue ? set.Count1 * set.Count2.Value : set.Count1;
            return (double)volume;
        });
    }

    /// <summary>
    /// Sequential method: Compare each workout to the previous one and average all improvements.
    /// Example: workout 2 vs 1, workout 3 vs 2, etc., then average.
    /// </summary>
    private static double CalculateSequentialImprovement(
        List<ExerciseHistoryReadModel> orderedHistories
    )
    {
        var improvements = new List<double>();

        for (int i = 1; i < orderedHistories.Count; i++)
        {
            var previousVolume = CalculateVolume(orderedHistories[i - 1].NewExerciseSets);
            var currentVolume = CalculateVolume(orderedHistories[i].NewExerciseSets);

            if (previousVolume > 0)
            {
                var improvement = ((currentVolume - previousVolume) / previousVolume) * 100.0;
                improvements.Add(improvement);
            }
        }

        return improvements.Count > 0 ? improvements.Average() : 0.0;
    }

    /// <summary>
    /// FirstVsLast method: Compare the first workout to the most recent one.
    /// </summary>
    private static double CalculateFirstVsLastImprovement(
        List<ExerciseHistoryReadModel> orderedHistories
    )
    {
        var firstVolume = CalculateVolume(orderedHistories[0].NewExerciseSets);
        var lastVolume = CalculateVolume(orderedHistories[^1].NewExerciseSets);

        if (firstVolume > 0)
        {
            return ((lastVolume - firstVolume) / firstVolume) * 100.0;
        }

        return 0.0;
    }
}
