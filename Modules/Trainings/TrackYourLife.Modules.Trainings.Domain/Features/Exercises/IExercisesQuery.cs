using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Trainings.Domain.Features.Exercises;

public interface IExercisesQuery
{
    Task<ExerciseReadModel?> GetByIdAsync(
        ExerciseId id,
        CancellationToken cancellationToken = default
    );

    Task<bool> ExistsAsync(ExerciseId id, CancellationToken cancellationToken = default);

    Task<bool> ExistsByUserIdAndNameAsync(
        UserId userId,
        string name,
        CancellationToken cancellationToken = default
    );

    Task<ExerciseReadModel?> GetByUserIdAndNameAsync(
        UserId userId,
        string name,
        CancellationToken cancellationToken = default
    );

    Task<IEnumerable<ExerciseReadModel>> GetEnumerableByUserIdAsync(
        UserId userId,
        CancellationToken cancellationToken = default
    );

    Task<IEnumerable<ExerciseReadModel>> GetEnumerableWithinIdsCollectionAsync(
        IEnumerable<ExerciseId> ids,
        CancellationToken cancellationToken = default
    );
}
