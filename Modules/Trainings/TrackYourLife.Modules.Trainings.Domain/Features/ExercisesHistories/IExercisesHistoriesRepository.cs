using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;

namespace TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;

public interface IExercisesHistoriesRepository
{
    Task<ExerciseHistory?> GetByIdAsync(
        ExerciseHistoryId id,
        CancellationToken cancellationToken
    );

    Task AddAsync(ExerciseHistory exerciseHistory, CancellationToken cancellationToken);

    Task<IEnumerable<ExerciseHistory>> GetByOngoingTrainingIdAsync(
        OngoingTrainingId ongoingTrainingId,
        CancellationToken cancellationToken
    );

    Task<IEnumerable<ExerciseHistory>> GetByOngoingTrainingIdAndAreNotAppliedAsync(
        OngoingTrainingId ongoingTrainingId,
        CancellationToken cancellationToken
    );

    Task<ExerciseHistory?> GetByOngoingTrainingIdAndExerciseIdAsync(
        OngoingTrainingId ongoingTrainingId,
        ExerciseId exerciseId,
        CancellationToken cancellationToken
    );

    void Update(ExerciseHistory exerciseHistory);

    void Remove(ExerciseHistory exerciseHistory);
}
