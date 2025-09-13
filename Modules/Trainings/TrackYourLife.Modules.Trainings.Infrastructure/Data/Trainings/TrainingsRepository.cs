using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.Modules.Trainings.Infrastructure.Data.Trainings.Specifications;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Trainings.Infrastructure.Data.Trainings;

public class TrainingsRepository(TrainingsWriteDbContext dbContext)
    : GenericRepository<Training, TrainingId>(
        dbContext.Trainings,
        CreateQuery(dbContext.Trainings)
    ),
        ITrainingsRepository
{
    private static IQueryable<Training> CreateQuery(DbSet<Training> dbSet)
    {
        return dbSet.Include(t => t.TrainingExercises).ThenInclude(te => te.Exercise);
    }

    public async Task<IEnumerable<Training>> GetThatContainsExerciseAsync(
        ExerciseId exerciseId,
        CancellationToken cancellationToken = default
    )
    {
        return await WhereAsync(
            new TrainingWithExerciseSpecification(exerciseId),
            cancellationToken
        );
    }
}
