using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.Modules.Trainings.Infrastructure.Data.ExercisesHistories.Specifications;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Trainings.Infrastructure.Data.ExercisesHistories;

internal sealed class ExercisesHistoriesQuery(TrainingsReadDbContext dbContext)
    : GenericQuery<ExerciseHistoryReadModel, ExerciseHistoryId>(dbContext.ExerciseHistories),
        IExercisesHistoriesQuery
{
    public async Task<IEnumerable<ExerciseHistoryReadModel>> GetByExerciseIdAsync(
        ExerciseId exerciseId,
        CancellationToken cancellationToken
    )
    {
        return (
            await WhereAsync(
                new ExerciseHistoryReadModelWithExerciseIdSpecification(exerciseId),
                cancellationToken
            )
        ).OrderByDescending(x => x.CreatedOnUtc);
    }
}
