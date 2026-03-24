using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Contracts.Dtos;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetWorkoutHistory;

public class GetWorkoutHistoryQueryHandler(
    IUserIdentifierProvider userIdentifierProvider,
    IOngoingTrainingsQuery ongoingTrainingsQuery,
    IExercisesHistoriesQuery exercisesHistoriesQuery
) : IQueryHandler<GetWorkoutHistoryQuery, IEnumerable<WorkoutHistoryDto>>
{
    public async Task<Result<IEnumerable<WorkoutHistoryDto>>> Handle(
        GetWorkoutHistoryQuery request,
        CancellationToken cancellationToken
    )
    {
        var userId = userIdentifierProvider.UserId;

        // Get completed workouts
        IEnumerable<OngoingTrainingReadModel> completedWorkouts;
        if (request.StartDate.HasValue || request.EndDate.HasValue)
        {
            var startDate = request.StartDate ?? DateOnly.MinValue;
            var endDate = request.EndDate ?? DateOnly.MaxValue;
            completedWorkouts = await ongoingTrainingsQuery.GetCompletedByUserIdAndDateRangeAsync(
                userId,
                startDate,
                endDate,
                cancellationToken
            );
        }
        else
        {
            completedWorkouts = await ongoingTrainingsQuery.GetCompletedByUserIdAsync(
                userId,
                cancellationToken
            );
        }

        var eligible = completedWorkouts
            .Where(w => w.FinishedOnUtc.HasValue && w.Training != null)
            .ToList();

        if (eligible.Count == 0)
        {
            return Result.Success<IEnumerable<WorkoutHistoryDto>>(Array.Empty<WorkoutHistoryDto>());
        }

        IEnumerable<ExerciseHistoryReadModel> exerciseHistoriesForScope;
        if (request.StartDate.HasValue || request.EndDate.HasValue)
        {
            var startDate = request.StartDate ?? DateOnly.MinValue;
            var endDate = request.EndDate ?? DateOnly.MaxValue;
            exerciseHistoriesForScope =
                await exercisesHistoriesQuery.GetCompletedByUserIdAndDateRangeAsync(
                    userId,
                    startDate,
                    endDate,
                    cancellationToken
                );
        }
        else
        {
            exerciseHistoriesForScope = await exercisesHistoriesQuery.GetCompletedByUserIdAsync(
                userId,
                cancellationToken
            );
        }

        var historiesByOngoingId = exerciseHistoriesForScope
            .GroupBy(eh => eh.OngoingTrainingId)
            .ToDictionary(g => g.Key, g => g.AsEnumerable());

        var workoutHistory = eligible
            .Select(w =>
            {
                var training = w.Training!;
                var finishedOnUtc = w.FinishedOnUtc!.Value;
                var totalExercises = training.TrainingExercises.Count;
                var histories = historiesByOngoingId.GetValueOrDefault(w.Id);
                var completedExercises =
                    histories == null
                        ? 0
                        : histories.Count(eh => eh.Status == ExerciseStatus.Completed);

                return new WorkoutHistoryDto(
                    Id: w.Id,
                    TrainingId: training.Id,
                    TrainingName: training.Name,
                    StartedOnUtc: w.StartedOnUtc,
                    FinishedOnUtc: finishedOnUtc,
                    DurationSeconds: (long)(finishedOnUtc - w.StartedOnUtc).TotalSeconds,
                    CaloriesBurned: w.CaloriesBurned,
                    CompletedExercisesCount: completedExercises,
                    TotalExercisesCount: totalExercises
                );
            })
            .OrderByDescending(w => w.FinishedOnUtc)
            .ToList();

        return Result.Success<IEnumerable<WorkoutHistoryDto>>(workoutHistory);
    }
}
