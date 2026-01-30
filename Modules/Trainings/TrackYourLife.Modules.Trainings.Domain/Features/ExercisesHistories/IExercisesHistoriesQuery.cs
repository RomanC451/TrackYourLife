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
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default
    );

    Task<IEnumerable<ExerciseHistoryReadModel>> GetByUserIdAsync(
        UserId userId,
        CancellationToken cancellationToken = default
    );
}
