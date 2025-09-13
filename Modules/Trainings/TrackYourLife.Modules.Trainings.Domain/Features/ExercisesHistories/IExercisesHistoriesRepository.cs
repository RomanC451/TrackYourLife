using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;

namespace TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;

public interface IExercisesHistoriesRepository
{
    Task AddAsync(ExerciseHistory exerciseHistory, CancellationToken cancellationToken);

    Task<IEnumerable<ExerciseHistory>> GetByOngoingTrainingIdAndAreNotAppliedAsync(
        OngoingTrainingId ongoingTrainingId,
        CancellationToken cancellationToken
    );

    void Update(ExerciseHistory exerciseHistory);
}
