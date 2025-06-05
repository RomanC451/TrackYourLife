using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.Modules.Trainings.Infrastructure.Data.Trainings.Specifications;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Trainings.Infrastructure.Data.Trainings;

public class TrainingsQuery(TrainingsReadDbContext dbContext)
    : GenericQuery<TrainingReadModel, TrainingId>(CreateQuery(dbContext.Trainings)),
        ITrainingsQuery
{
    private static IQueryable<TrainingReadModel> CreateQuery(DbSet<TrainingReadModel> dbSet)
    {
        return dbSet.Include(t => t.TrainingExercises).ThenInclude(te => te.Exercise);
    }

    public async Task<IEnumerable<TrainingReadModel>> GetTrainingsByUserIdAsync(
        UserId userId,
        CancellationToken cancellationToken = default
    )
    {
        return await WhereAsync(
            new TrainingReadModelWithUserIdSpecification(userId),
            cancellationToken
        );
    }

    public async Task<bool> ExistsByUserIdAndNameAsync(
        UserId userId,
        string name,
        CancellationToken cancellationToken = default
    )
    {
        return await AnyAsync(
            new TrainingWithUserIdAndNameSpecification(userId, name),
            cancellationToken
        );
    }
}
