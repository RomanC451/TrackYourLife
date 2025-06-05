using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;

namespace TrackYourLife.Modules.Trainings.Domain.Features.Trainings;

public interface ITrainingsRepository
{
    Task<Training?> GetByIdAsync(TrainingId id, CancellationToken cancellationToken = default);

    Task<IEnumerable<Training>> GetThatContainsExerciseAsync(
        ExerciseId exerciseId,
        CancellationToken cancellationToken = default
    );

    Task AddAsync(Training training, CancellationToken cancellationToken = default);

    void Remove(Training training);

    void Update(Training training);

    void UpdateRange(IEnumerable<Training> trainings);
}
