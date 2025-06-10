namespace TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;

public interface IExercisesHistoriesRepository
{
    Task AddAsync(ExerciseHistory exerciseHistory, CancellationToken cancellationToken);
}
