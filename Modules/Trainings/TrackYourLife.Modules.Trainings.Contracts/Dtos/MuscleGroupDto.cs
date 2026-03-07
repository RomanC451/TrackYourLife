using TrackYourLife.Modules.Trainings.Domain.Features.MuscleGroups;

namespace TrackYourLife.Modules.Trainings.Contracts.Dtos;

public sealed record MuscleGroupDto(
    MuscleGroupId Id,
    string Name,
    MuscleGroupId? ParentMuscleGroupId,
    IReadOnlyList<MuscleGroupDto> Children
);
