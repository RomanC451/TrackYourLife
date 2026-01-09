using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.Modules.Trainings.Infrastructure.Data.ExercisesHistories.Specifications;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Trainings.Infrastructure.Data.ExercisesHistories;

internal sealed class ExercisesHistoriesRepository(TrainingsWriteDbContext dbContext)
    : GenericRepository<ExerciseHistory, ExerciseHistoryId>(dbContext.ExerciseHistories),
        IExercisesHistoriesRepository
{
    public async Task<IEnumerable<ExerciseHistory>> GetByOngoingTrainingIdAsync(
        OngoingTrainingId ongoingTrainingId,
        CancellationToken cancellationToken
    )
    {
        return await WhereAsync(
            new ExerciseHistoryWithOngoingTrainingIdSpecification(ongoingTrainingId),
            cancellationToken
        );
    }

    public async Task<IEnumerable<ExerciseHistory>> GetByOngoingTrainingIdAndAreNotAppliedAsync(
        OngoingTrainingId ongoingTrainingId,
        CancellationToken cancellationToken
    )
    {
        return (
            await WhereAsync(
                new ExerciseHistoryWithOngoingTrainingIdAndNotAppliedSpecification(
                    ongoingTrainingId
                ),
                cancellationToken
            )
        ).OrderByDescending(x => x.CreatedOnUtc);
    }

    public async Task<ExerciseHistory?> GetByOngoingTrainingIdAndExerciseIdAsync(
        OngoingTrainingId ongoingTrainingId,
        ExerciseId exerciseId,
        CancellationToken cancellationToken
    )
    {
        return await FirstOrDefaultAsync(
            new ExerciseHistoryWithOngoingTrainingIdAndExerciseIdSpecification(
                ongoingTrainingId,
                exerciseId
            ),
            cancellationToken
        );
    }
}
