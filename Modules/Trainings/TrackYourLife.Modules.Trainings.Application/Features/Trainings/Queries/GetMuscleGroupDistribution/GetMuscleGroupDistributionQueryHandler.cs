using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Contracts.Dtos;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.Modules.Trainings.Domain.Features.MuscleGroups;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetMuscleGroupDistribution;

public class GetMuscleGroupDistributionQueryHandler(
    IUserIdentifierProvider userIdentifierProvider,
    IExercisesHistoriesQuery exercisesHistoriesQuery,
    IExercisesQuery exercisesQuery,
    IMuscleGroupsQuery muscleGroupsQuery
) : IQueryHandler<GetMuscleGroupDistributionQuery, IReadOnlyList<MuscleGroupDistributionDto>>
{
    public async Task<Result<IReadOnlyList<MuscleGroupDistributionDto>>> Handle(
        GetMuscleGroupDistributionQuery request,
        CancellationToken cancellationToken
    )
    {
        var userId = userIdentifierProvider.UserId;

        IEnumerable<ExerciseHistoryReadModel> completedHistories;
        if (request.StartDate.HasValue || request.EndDate.HasValue)
        {
            var startDate = request.StartDate ?? DateOnly.MinValue;
            var endDate = request.EndDate ?? DateOnly.MaxValue;
            completedHistories =
                await exercisesHistoriesQuery.GetCompletedByUserIdAndDateRangeAsync(
                    userId,
                    startDate,
                    endDate,
                    cancellationToken
                );
        }
        else
        {
            completedHistories = await exercisesHistoriesQuery.GetCompletedByUserIdAsync(
                userId,
                cancellationToken
            );
        }

        var historyList = completedHistories.ToList();
        if (historyList.Count == 0)
        {
            return Result.Success<IReadOnlyList<MuscleGroupDistributionDto>>(
                new List<MuscleGroupDistributionDto>()
            );
        }

        var exerciseIds = historyList.Select(h => h.ExerciseId).Distinct().ToList();
        var exercises = await exercisesQuery.GetEnumerableWithinIdsCollectionAsync(
            exerciseIds,
            cancellationToken
        );
        var exerciseById = exercises.ToDictionary(e => e.Id);

        var allMuscleGroups = await muscleGroupsQuery.GetAllAsync(cancellationToken);
        var parentId = ResolveParentId(allMuscleGroups, request.ParentMuscleGroupName);

        var muscleGroupCounts = new Dictionary<string, int>();
        foreach (var history in historyList)
        {
            if (!exerciseById.TryGetValue(history.ExerciseId, out var exercise))
                continue;

            var setsCount = history.NewExerciseSets?.Count ?? 0;
            var weight = setsCount > 0 ? setsCount : 1; // If no sets recorded, count as 1 occurrence

            var muscleGroups = exercise.MuscleGroups ?? new List<string>();
            foreach (var name in muscleGroups)
            {
                var key = ResolveDistributionKey(name, allMuscleGroups, parentId);
                if (key is null)
                    continue;

                if (!muscleGroupCounts.ContainsKey(key))
                    muscleGroupCounts[key] = 0;
                muscleGroupCounts[key] += weight;
            }
        }

        var totalMuscleGroupWorkouts = muscleGroupCounts.Values.Sum();
        var muscleGroupDistribution = muscleGroupCounts
            .Select(kvp => new MuscleGroupDistributionDto(
                kvp.Key,
                kvp.Value,
                totalMuscleGroupWorkouts > 0
                    ? (double)kvp.Value / totalMuscleGroupWorkouts * 100
                    : 0
            ))
            .OrderByDescending(m => m.WorkoutCount)
            .ToList();

        return Result.Success<IReadOnlyList<MuscleGroupDistributionDto>>(muscleGroupDistribution);
    }

    /// <summary>
    /// Resolves parent muscle group name to id. Returns null if name is null/empty or no group matches.
    /// </summary>
    private static MuscleGroupId? ResolveParentId(
        IReadOnlyList<MuscleGroup> all,
        string? parentMuscleGroupName
    )
    {
        if (string.IsNullOrWhiteSpace(parentMuscleGroupName))
            return null;

        var match = all.FirstOrDefault(m =>
            string.Equals(m.Name, parentMuscleGroupName, StringComparison.OrdinalIgnoreCase)
        );
        return match?.Id;
    }

    /// <summary>
    /// Returns the key to use for distribution counting.
    /// When parentId is null: returns the main (root) muscle group name for the given name, or the name itself if unknown.
    /// When parentId is set: returns the name only if it is a direct subgroup of that parent; otherwise null (excluded).
    /// </summary>
    private static string? ResolveDistributionKey(
        string muscleGroupName,
        IReadOnlyList<MuscleGroup> all,
        MuscleGroupId? parentId
    )
    {
        var byName = all.ToDictionary(m => m.Name, StringComparer.OrdinalIgnoreCase);
        if (!byName.TryGetValue(muscleGroupName, out var group))
        {
            // Unknown name: when no parent filter, count as its own main group; when filtering by parent, exclude
            return parentId is null ? muscleGroupName : null;
        }

        if (parentId is null)
        {
            // Default: main groups only — resolve to root name
            var current = group;
            while (current.ParentMuscleGroupId is { } pid)
                current = all.First(m => m.Id == pid);
            return current.Name;
        }

        // Subgroup distribution: include only direct children of the requested parent
        return group.ParentMuscleGroupId == parentId ? group.Name : null;
    }
}
