using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Contracts.Dtos;
using TrackYourLife.Modules.Trainings.Domain.Features.MuscleGroups;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.Features.MuscleGroups.Queries.GetMuscleGroups;

public sealed class GetMuscleGroupsQueryHandler(IMuscleGroupsQuery muscleGroupsQuery)
    : IQueryHandler<GetMuscleGroupsQuery, IReadOnlyList<MuscleGroupDto>>
{
    public async Task<Result<IReadOnlyList<MuscleGroupDto>>> Handle(
        GetMuscleGroupsQuery request,
        CancellationToken cancellationToken
    )
    {
        var all = await muscleGroupsQuery.GetAllAsync(cancellationToken);
        var tree = BuildTree(all, parentId: null);
        return Result.Success(tree);
    }

    private static IReadOnlyList<MuscleGroupDto> BuildTree(
        IReadOnlyList<MuscleGroup> all,
        MuscleGroupId? parentId
    )
    {
        return all.Where(m => m.ParentMuscleGroupId == parentId)
            .Select(m => new MuscleGroupDto(
                m.Id,
                m.Name,
                m.ParentMuscleGroupId,
                BuildTree(all, m.Id)
            ))
            .ToList();
    }
}
