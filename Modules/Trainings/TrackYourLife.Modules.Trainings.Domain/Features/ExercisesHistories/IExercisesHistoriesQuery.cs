using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;

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
}
