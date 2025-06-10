using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Infrastructure.Data.Exercises.Specifications;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Trainings.Infrastructure.Data.Exercises;

internal sealed class ExercisesQuery(TrainingsReadDbContext dbContext)
    : GenericQuery<ExerciseReadModel, ExerciseId>(dbContext.Exercises),
        IExercisesQuery
{
    public async Task<bool> ExistsByUserIdAndNameAsync(
        UserId userId,
        string name,
        CancellationToken cancellationToken = default
    )
    {
        return await AnyAsync(
            new ExerciseReadModelWithUserIdAndNameSpecification(userId, name),
            cancellationToken
        );
    }

    public async Task<ExerciseReadModel?> GetByUserIdAndNameAsync(
        UserId userId,
        string name,
        CancellationToken cancellationToken = default
    )
    {
        return await FirstOrDefaultAsync(
            new ExerciseReadModelWithUserIdAndNameSpecification(userId, name),
            cancellationToken
        );
    }

    public async Task<IEnumerable<ExerciseReadModel>> GetEnumerableByUserIdAsync(
        UserId userId,
        CancellationToken cancellationToken = default
    )
    {
        return (
            await WhereAsync(
                new ExerciseReadModelWithUserIdSpecification(userId),
                cancellationToken
            )
        ).OrderBy(e => e.CreatedOnUtc);
    }

    public async Task<IEnumerable<ExerciseReadModel>> GetEnumerableWithinIdsCollectionAsync(
        IEnumerable<ExerciseId> ids,
        CancellationToken cancellationToken = default
    )
    {
        return await WhereAsync(
            new ExerciseReadModelWithinIdsCollectionSpecification(ids),
            cancellationToken
        );
    }
}
