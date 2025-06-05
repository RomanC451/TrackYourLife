using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.Modules.Trainings.Infrastructure.Data.OngoingTrainings.Specifications;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Trainings.Infrastructure.Data.OngoingTrainings;

internal sealed class OngoingTrainingsRepository(TrainingsWriteDbContext context)
    : GenericRepository<OngoingTraining, OngoingTrainingId>(
        context.OngoingTrainings,
        CreateQuery(context.OngoingTrainings)
    ),
        IOngoingTrainingsRepository
{
    private static IQueryable<OngoingTraining> CreateQuery(DbSet<OngoingTraining> dbSet)
    {
        return dbSet
            .Include(ot => ot.Training)
            .ThenInclude(t => t.TrainingExercises)
            .ThenInclude(te => te.Exercise);
    }

    public async Task<OngoingTraining?> GetUnfinishedByUserIdAsync(
        UserId userId,
        CancellationToken cancellationToken = default
    )
    {
        return await FirstOrDefaultAsync(
            new OngoingTrainingWithUserIdAndFinishedOnUtcSpecification(userId, null),
            cancellationToken
        );
    }
}
