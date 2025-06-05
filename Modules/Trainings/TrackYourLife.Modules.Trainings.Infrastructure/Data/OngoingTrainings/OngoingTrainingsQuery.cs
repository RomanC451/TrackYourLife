using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.Modules.Trainings.Infrastructure.Data.OngoingTrainings.Specifications;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Trainings.Infrastructure.Data.OngoingTrainings;

public class OngoingTrainingsQuery(TrainingsReadDbContext context)
    : GenericQuery<OngoingTrainingReadModel, OngoingTrainingId>(
        CreateQuery(context.OngoingTrainings)
    ),
        IOngoingTrainingsQuery
{
    private static IQueryable<OngoingTrainingReadModel> CreateQuery(
        DbSet<OngoingTrainingReadModel> dbSet
    )
    {
        return dbSet
            .Include(ot => ot.Training)
            .ThenInclude(t => t.TrainingExercises)
            .ThenInclude(te => te.Exercise);
    }

    public async Task<OngoingTrainingReadModel?> GetUnfinishedByUserIdAsync(
        UserId userId,
        CancellationToken cancellationToken = default
    )
    {
        return await FirstOrDefaultAsync(
            new OngoingTrainingReadModelWithUserIdAndFinishedOnUtcSpecification(userId, null),
            cancellationToken
        );
    }
}
