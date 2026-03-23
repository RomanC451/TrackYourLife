using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Trainings.Domain.Features.WorkoutPlans;
using TrackYourLife.Modules.Trainings.Infrastructure.Data.WorkoutPlans.Specifications;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Trainings.Infrastructure.Data.WorkoutPlans;

public sealed class WorkoutPlansQuery(TrainingsReadDbContext dbContext)
    : GenericQuery<WorkoutPlanReadModel, WorkoutPlanId>(CreateQuery(dbContext.WorkoutPlans)),
        IWorkoutPlansQuery
{
    private static IQueryable<WorkoutPlanReadModel> CreateQuery(DbSet<WorkoutPlanReadModel> dbSet)
    {
        return dbSet
            .Include(wp => wp.WorkoutPlanTrainings)
            .ThenInclude(wpt => wpt.Training)
            .ThenInclude(t => t.TrainingExercises)
            .ThenInclude(te => te.Exercise);
    }

    public async Task<WorkoutPlanReadModel?> GetActiveByUserIdAsync(
        UserId userId,
        CancellationToken cancellationToken = default
    )
    {
        return await FirstOrDefaultAsync(
            new WorkoutPlanReadModelWithUserIdAndActiveSpecification(userId),
            cancellationToken
        );
    }

    public async Task<IEnumerable<WorkoutPlanReadModel>> GetByUserIdAsync(
        UserId userId,
        CancellationToken cancellationToken = default
    )
    {
        return await WhereAsync(new WorkoutPlanReadModelWithUserIdSpecification(userId), cancellationToken);
    }
}
