using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Trainings.Domain.Features.MuscleGroups;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Trainings.Infrastructure.Data.MuscleGroups;

internal sealed class MuscleGroupsQuery(TrainingsWriteDbContext dbContext)
    : GenericQuery<MuscleGroup, MuscleGroupId>(dbContext.MuscleGroups),
        IMuscleGroupsQuery
{
    public async Task<IReadOnlyList<MuscleGroup>> GetAllAsync(
        CancellationToken cancellationToken = default
    )
    {
        return await query.OrderBy(m => m.Name).ToListAsync(cancellationToken);
    }
}
