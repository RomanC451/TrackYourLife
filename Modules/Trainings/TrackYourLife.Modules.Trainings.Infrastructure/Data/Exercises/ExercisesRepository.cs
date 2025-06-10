using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Infrastructure.Data.Exercises.Specifications;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Trainings.Infrastructure.Data.Exercises;

internal sealed class ExercisesRepository(TrainingsWriteDbContext dbContext)
    : GenericRepository<Exercise, ExerciseId>(dbContext.Exercises),
        IExercisesRepository
{
    public async Task<IEnumerable<Exercise>> GetEnumerableWithinIdsCollectionAsync(
        IEnumerable<ExerciseId> ids,
        CancellationToken cancellationToken = default
    )
    {
        return await WhereAsync(
            new ExerciseWithinIdsCollectionSpecification(ids),
            cancellationToken
        );
    }
}
