using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;

public interface IExercisesHistoriesQuery
{
    Task<IEnumerable<ExerciseHistoryReadModel>> GetByExerciseIdAsync(
        ExerciseId exerciseId,
        CancellationToken cancellationToken
    );

    Task<IEnumerable<ExerciseHistoryReadModel>> GetByOngoingTrainingIdAsync(
        OngoingTrainingId ongoingTrainingId,
        CancellationToken cancellationToken
    );

    Task<IEnumerable<ExerciseHistoryReadModel>> GetByUserIdAndDateRangeAsync(
        UserId userId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken = default
    );

    Task<IEnumerable<ExerciseHistoryReadModel>> GetByUserIdAsync(
        UserId userId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Returns exercise histories only for completed workouts (FinishedOnUtc has value).
    /// </summary>
    Task<IEnumerable<ExerciseHistoryReadModel>> GetCompletedByUserIdAsync(
        UserId userId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Returns exercise histories for completed workouts whose FinishedOnUtc falls within [startDate, endDate].
    /// </summary>
    Task<IEnumerable<ExerciseHistoryReadModel>> GetCompletedByUserIdAndDateRangeAsync(
        UserId userId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken = default
    );
}
