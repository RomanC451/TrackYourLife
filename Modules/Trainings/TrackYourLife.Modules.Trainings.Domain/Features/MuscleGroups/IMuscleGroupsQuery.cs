namespace TrackYourLife.Modules.Trainings.Domain.Features.MuscleGroups;

public interface IMuscleGroupsQuery
{
    Task<IReadOnlyList<MuscleGroup>> GetAllAsync(CancellationToken cancellationToken = default);
}
