namespace TrackYourLife.Modules.Trainings.Domain.Features.Exercises;

public interface IExerciseRepository
{
    Task<Exercise?> GetByIdAsync(ExerciseId id, CancellationToken cancellationToken = default);

    Task<IEnumerable<Exercise>> GetEnumerableWithinIdsCollectionAsync(
        IEnumerable<ExerciseId> ids,
        CancellationToken cancellationToken = default
    );

    Task AddAsync(Exercise exercise, CancellationToken cancellationToken = default);

    void Remove(Exercise exercise);

    void Update(Exercise exercise);
}
