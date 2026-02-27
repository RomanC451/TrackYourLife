using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
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

    public async Task<bool> IsTrainingOngoingAsync(
        TrainingId trainingId,
        CancellationToken cancellationToken = default
    )
    {
        return await AnyAsync(
            new OngoingTrainingReadModelWithTrainingIdAndNotFinishedSpecification(trainingId),
            cancellationToken
        );
    }

    public async Task<IEnumerable<OngoingTrainingReadModel>> GetCompletedByUserIdAndDateRangeAsync(
        UserId userId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken = default
    )
    {
        // Half-open interval [startOfStartDay, startOfDayAfterEnd) so the full end date is included
        var startInclusive = DateTime.SpecifyKind(
            startDate.ToDateTime(TimeOnly.MinValue),
            DateTimeKind.Utc
        );
        var endExclusive = DateTime.SpecifyKind(
            endDate.AddDays(1).ToDateTime(TimeOnly.MinValue),
            DateTimeKind.Utc
        );
        return await WhereAsync(
            new OngoingTrainingReadModelWithUserIdAndDateRangeSpecification(
                userId,
                startInclusive,
                endExclusive
            ),
            cancellationToken
        );
    }

    public async Task<IEnumerable<OngoingTrainingReadModel>> GetCompletedByUserIdAsync(
        UserId userId,
        CancellationToken cancellationToken = default
    )
    {
        return await WhereAsync(
            new OngoingTrainingReadModelWithUserIdAndCompletedSpecification(userId),
            cancellationToken
        );
    }
}
