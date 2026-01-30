using Microsoft.Extensions.Caching.Memory;
using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Contracts.Dtos;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Contracts.Common;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;
using ExerciseStatus = TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories.ExerciseStatus;

namespace TrackYourLife.Modules.Trainings.Application.Features.Exercises.Queries.GetTopExercises;

public class GetTopExercisesQueryHandler(
    IUserIdentifierProvider userIdentifierProvider,
    IExercisesHistoriesQuery exercisesHistoriesQuery,
    IExercisesQuery exercisesQuery,
    IMemoryCache memoryCache
) : IQueryHandler<GetTopExercisesQuery, PagedList<TopExerciseDto>>
{
    private const string CacheKeyPrefix = "TopExercises_";
    private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(5);

    public async Task<Result<PagedList<TopExerciseDto>>> Handle(
        GetTopExercisesQuery request,
        CancellationToken cancellationToken
    )
    {
        var userId = userIdentifierProvider.UserId;
        List<TopExerciseDto> topExercises;

        if (request.StartDate.HasValue && request.EndDate.HasValue)
        {
            var startDate = DateTime.SpecifyKind(request.StartDate.Value, DateTimeKind.Utc);
            var endDate = DateTime.SpecifyKind(request.EndDate.Value, DateTimeKind.Utc);
            topExercises = await CreateTopExercisesListAsync(
                userId,
                startDate,
                endDate,
                cancellationToken
            );
        }
        else
        {
            var cacheKey = $"{CacheKeyPrefix}{userId.Value}";
            topExercises =
                await memoryCache.GetOrCreateAsync(
                    cacheKey,
                    async entry =>
                    {
                        entry.AbsoluteExpirationRelativeToNow = CacheExpiration;
                        return await CreateTopExercisesListAsync(
                            userId,
                            null,
                            null,
                            cancellationToken
                        );
                    }
                ) ?? [];
        }

        var pagedListResult = PagedList<TopExerciseDto>.Create(
            topExercises,
            request.Page,
            request.PageSize
        );

        if (pagedListResult.IsFailure)
        {
            return Result.Failure<PagedList<TopExerciseDto>>(pagedListResult.Error);
        }

        return Result.Success(pagedListResult.Value);
    }

    private async Task<List<TopExerciseDto>> CreateTopExercisesListAsync(
        UserId userId,
        DateTime? startDate,
        DateTime? endDate,
        CancellationToken cancellationToken
    )
    {
        IEnumerable<ExerciseHistoryReadModel> exerciseHistories =
            startDate.HasValue && endDate.HasValue
                ? await exercisesHistoriesQuery.GetByUserIdAndDateRangeAsync(
                    userId,
                    startDate.Value,
                    endDate.Value,
                    cancellationToken
                )
                : await exercisesHistoriesQuery.GetByUserIdAsync(userId, cancellationToken);

        // Group by exercise ID
        var exerciseStats = exerciseHistories
            .GroupBy(eh => eh.ExerciseId)
            .Select(g => new
            {
                ExerciseId = g.Key,
                CompletionCount = g.Count(eh => eh.Status == ExerciseStatus.Completed),
                SkipCount = g.Count(eh => eh.Status == ExerciseStatus.Skipped),
                Histories = g.ToList(),
            })
            .ToList();

        // Get exercise details
        var exerciseIds = exerciseStats.Select(s => s.ExerciseId).ToList();
        var exercises = await exercisesQuery.GetEnumerableWithinIdsCollectionAsync(
            exerciseIds,
            cancellationToken
        );
        var exerciseDict = exercises.ToDictionary(e => e.Id, e => e.Name);

        // Calculate improvement percentage (simplified: compare old vs new sets)
        var topExercises = exerciseStats
            .Select(stat =>
            {
                var exerciseName = exerciseDict.GetValueOrDefault(stat.ExerciseId, "Unknown");

                // Calculate improvement: count how many times sets increased
                var improvements = stat
                    .Histories.Where(h => h.Status == ExerciseStatus.Completed)
                    .Count(h =>
                    {
                        var oldTotal = h.OldExerciseSets.Sum(s => s.Count1);
                        var newTotal = h.NewExerciseSets.Sum(s => s.Count1);
                        return newTotal > oldTotal;
                    });

                var totalCompleted = stat.CompletionCount;
                var improvementPercentage =
                    totalCompleted > 0 ? (double)improvements / totalCompleted * 100 : 0;

                return new TopExerciseDto(
                    ExerciseId: stat.ExerciseId,
                    ExerciseName: exerciseName,
                    CompletionCount: stat.CompletionCount,
                    SkipCount: stat.SkipCount,
                    ImprovementPercentage: improvementPercentage
                );
            })
            .OrderByDescending(e => e.CompletionCount)
            .ThenByDescending(e => e.ImprovementPercentage)
            .ToList();

        return topExercises;
    }
}
