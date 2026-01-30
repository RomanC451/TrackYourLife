using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.Modules.Trainings.Infrastructure.Data.ExercisesHistories.Specifications;
using TrackYourLife.SharedLib.Domain.Ids;
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

    public async Task<IEnumerable<ExerciseHistoryReadModel>> GetByOngoingTrainingIdAsync(
        OngoingTrainingId ongoingTrainingId,
        CancellationToken cancellationToken
    )
    {
        return await WhereAsync(
            new ExerciseHistoryReadModelWithOngoingTrainingIdSpecification(ongoingTrainingId),
            cancellationToken
        );
    }

    public async Task<IEnumerable<ExerciseHistoryReadModel>> GetByUserIdAndDateRangeAsync(
        UserId userId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default
    )
    {
        // Join with OngoingTrainings to filter by UserId
        var query = dbContext.ExerciseHistories
            .Join(
                dbContext.OngoingTrainings,
                eh => eh.OngoingTrainingId,
                ot => ot.Id,
                (eh, ot) => new { ExerciseHistory = eh, OngoingTraining = ot }
            )
            .Where(x => x.OngoingTraining.UserId == userId
                && x.ExerciseHistory.CreatedOnUtc >= startDate
                && x.ExerciseHistory.CreatedOnUtc <= endDate)
            .Select(x => x.ExerciseHistory);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ExerciseHistoryReadModel>> GetByUserIdAsync(
        UserId userId,
        CancellationToken cancellationToken = default
    )
    {
        // Join with OngoingTrainings to filter by UserId
        var query = dbContext.ExerciseHistories
            .Join(
                dbContext.OngoingTrainings,
                eh => eh.OngoingTrainingId,
                ot => ot.Id,
                (eh, ot) => new { ExerciseHistory = eh, OngoingTraining = ot }
            )
            .Where(x => x.OngoingTraining.UserId == userId)
            .Select(x => x.ExerciseHistory);

        return await query.ToListAsync(cancellationToken);
    }
}
