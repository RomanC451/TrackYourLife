using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Contracts.Dtos;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetWorkoutHistory;

public class GetWorkoutHistoryQueryHandler(
    IUserIdentifierProvider userIdentifierProvider,
    IOngoingTrainingsQuery ongoingTrainingsQuery
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
            var startDate = request.StartDate.HasValue
                ? DateTime.SpecifyKind(request.StartDate.Value, DateTimeKind.Utc)
                : DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc);
            var endDate = request.EndDate.HasValue
                ? DateTime.SpecifyKind(request.EndDate.Value, DateTimeKind.Utc)
                : DateTime.SpecifyKind(DateTime.MaxValue, DateTimeKind.Utc);
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

        var workoutHistory = completedWorkouts
            .Where(w => w.FinishedOnUtc.HasValue && w.Training != null)
            .Select(w => new WorkoutHistoryDto(
                Id: w.Id,
                TrainingId: w.Training.Id,
                TrainingName: w.Training.Name,
                StartedOnUtc: w.StartedOnUtc,
                FinishedOnUtc: w.FinishedOnUtc!.Value,
                DurationSeconds: (long)(w.FinishedOnUtc.Value - w.StartedOnUtc).TotalSeconds,
                CaloriesBurned: w.CaloriesBurned
            ))
            .OrderByDescending(w => w.FinishedOnUtc)
            .ToList();

        return Result.Success<IEnumerable<WorkoutHistoryDto>>(workoutHistory);
    }
}
