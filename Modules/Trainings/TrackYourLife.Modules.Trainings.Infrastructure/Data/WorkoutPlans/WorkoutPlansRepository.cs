using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Trainings.Domain.Features.WorkoutPlans;
using TrackYourLife.Modules.Trainings.Infrastructure.Data.WorkoutPlans.Specifications;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Trainings.Infrastructure.Data.WorkoutPlans;

public sealed class WorkoutPlansRepository(TrainingsWriteDbContext dbContext)
    : GenericRepository<WorkoutPlan, WorkoutPlanId>(
        dbContext.WorkoutPlans,
        CreateQuery(dbContext.WorkoutPlans)
    ),
        IWorkoutPlansRepository
{
    private static IQueryable<WorkoutPlan> CreateQuery(DbSet<WorkoutPlan> dbSet)
    {
        return dbSet.Include(wp => wp.WorkoutPlanTrainings);
    }

    public async Task<WorkoutPlan?> GetActiveByUserIdAsync(
        UserId userId,
        CancellationToken cancellationToken = default
    )
    {
        return await FirstOrDefaultAsync(
            new WorkoutPlanWithUserIdAndActiveSpecification(userId),
            cancellationToken
        );
    }
}
