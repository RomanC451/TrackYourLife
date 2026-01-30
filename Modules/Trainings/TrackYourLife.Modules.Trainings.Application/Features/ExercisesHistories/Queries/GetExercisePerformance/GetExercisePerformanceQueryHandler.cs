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

        // Group by exercise and calculate improvement from new vs old sets per session
        var performanceData = exerciseHistories
            .Where(eh => eh.Status == ExerciseStatus.Completed) // Only completed exercises
            .GroupBy(eh => eh.ExerciseId)
            .Select(group =>
            {
                var exerciseId = group.Key;
                var exerciseName = exerciseDict.GetValueOrDefault(exerciseId, "Unknown");
                var completedHistories = group.ToList();

                double improvementPercentage = CalculatePerSessionNewVsOldImprovement(
                    completedHistories
                );

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
    /// Calculates improvement by comparing new sets vs old sets within each session.
    /// For each session: improvement = (volume(NewExerciseSets) - volume(OldExerciseSets)) / volume(OldExerciseSets) * 100.
    /// Returns the average improvement across all sessions that have old volume &gt; 0.
    /// </summary>
    private static double CalculatePerSessionNewVsOldImprovement(
        List<ExerciseHistoryReadModel> histories
    )
    {
        var improvements = new List<double>();

        foreach (var history in histories)
        {
            var oldVolume = CalculateVolume(history.OldExerciseSets);
            var newVolume = CalculateVolume(history.NewExerciseSets);

            if (oldVolume > 0)
            {
                var improvement = ((newVolume - oldVolume) / oldVolume) * 100.0;
                improvements.Add(improvement);
            }
        }

        return improvements.Count > 0 ? improvements.Average() : 0.0;
    }
}
