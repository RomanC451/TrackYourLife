using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Contracts.Dtos;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetTrainingTemplatesUsage;

public class GetTrainingTemplatesUsageQueryHandler(
    IUserIdentifierProvider userIdentifierProvider,
    IOngoingTrainingsQuery ongoingTrainingsQuery,
    IExercisesHistoriesQuery exercisesHistoriesQuery
) : IQueryHandler<GetTrainingTemplatesUsageQuery, IEnumerable<TrainingTemplateUsageDto>>
{
    public async Task<Result<IEnumerable<TrainingTemplateUsageDto>>> Handle(
        GetTrainingTemplatesUsageQuery request,
        CancellationToken cancellationToken
    )
    {
        var userId = userIdentifierProvider.UserId;

        IEnumerable<OngoingTrainingReadModel> completedWorkoutsEnumerable =
            request.StartDate.HasValue && request.EndDate.HasValue
                ? await ongoingTrainingsQuery.GetCompletedByUserIdAndDateRangeAsync(
                    userId,
                    request.StartDate.Value,
                    request.EndDate.Value,
                    cancellationToken
                )
                : await ongoingTrainingsQuery.GetCompletedByUserIdAsync(userId, cancellationToken);

        var completedWorkouts = completedWorkoutsEnumerable.ToList();

        // Get exercise histories for the user to determine which completed workouts had skipped exercises
        var completedOngoingTrainingIds = completedWorkouts.Select(ot => ot.Id).ToHashSet();
        var allUserExerciseHistories = await exercisesHistoriesQuery.GetByUserIdAsync(
            userId,
            cancellationToken
        );
        var historiesByOngoingTrainingId = allUserExerciseHistories
            .Where(eh => completedOngoingTrainingIds.Contains(eh.OngoingTrainingId))
            .GroupBy(eh => eh.OngoingTrainingId)
            .ToDictionary(g => g.Key, g => g.ToList());

        // For each completed workout, check if it had any skipped exercises
        var ongoingTrainingIdToHasSkipped = completedOngoingTrainingIds.ToDictionary(
            id => id,
            id =>
            {
                if (!historiesByOngoingTrainingId.TryGetValue(id, out var histories))
                    return false;
                return histories.Any(eh => eh.Status == ExerciseStatus.Skipped);
            }
        );

        // Group by training ID
        var trainingStats = completedWorkouts
            .Where(ot => ot.Training != null)
            .GroupBy(ot => ot.Training!.Id)
            .Select(g => new
            {
                TrainingId = g.Key,
                TrainingName = g.First().Training!.Name,
                TotalCompleted = g.Count(),
                TotalFullyCompleted = g.Count(ot => !ongoingTrainingIdToHasSkipped[ot.Id]),
                TotalWithSkippedExercises = g.Count(ot => ongoingTrainingIdToHasSkipped[ot.Id]),
                CompletedWorkouts = g.ToList(),
            })
            .ToList();

        // Calculate statistics
        var templateUsage = trainingStats
            .Select(stat =>
            {
                var completionRate =
                    stat.TotalCompleted > 0
                        ? (double)stat.TotalFullyCompleted / stat.TotalCompleted * 100
                        : 0;

                var averageDuration = stat.CompletedWorkouts.Any()
                    ? stat.CompletedWorkouts.Average(w =>
                        (w.FinishedOnUtc!.Value - w.StartedOnUtc).TotalSeconds
                    )
                    : 0;

                var averageCalories = stat
                    .CompletedWorkouts.Where(w => w.CaloriesBurned.HasValue)
                    .Select(w => w.CaloriesBurned!.Value)
                    .DefaultIfEmpty(0)
                    .Average();

                return new TrainingTemplateUsageDto(
                    TrainingId: stat.TrainingId,
                    TrainingName: stat.TrainingName,
                    TotalCompleted: stat.TotalCompleted,
                    TotalFullyCompleted: stat.TotalFullyCompleted,
                    TotalWithSkippedExercises: stat.TotalWithSkippedExercises,
                    CompletionRate: completionRate,
                    AverageDurationSeconds: averageDuration,
                    AverageCaloriesBurned: averageCalories > 0 ? (int)averageCalories : null
                );
            })
            .OrderByDescending(t => t.TotalCompleted)
            .ThenByDescending(t => t.CompletionRate)
            .ToList();

        return Result.Success<IEnumerable<TrainingTemplateUsageDto>>(templateUsage);
    }
}
