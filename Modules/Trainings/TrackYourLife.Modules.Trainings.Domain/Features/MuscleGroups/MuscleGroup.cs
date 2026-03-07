using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.Modules.Trainings.Domain.Features.MuscleGroups;

public sealed class MuscleGroup : Entity<MuscleGroupId>, IReadModel<MuscleGroupId>
{
    public string Name { get; private set; } = string.Empty;
    public MuscleGroupId? ParentMuscleGroupId { get; private set; }

    private MuscleGroup()
        : base() { }

    private MuscleGroup(MuscleGroupId id, string name, MuscleGroupId? parentMuscleGroupId)
        : base(id)
    {
        Name = name;
        ParentMuscleGroupId = parentMuscleGroupId;
    }

    public static MuscleGroup Create(
        MuscleGroupId id,
        string name,
        MuscleGroupId? parentMuscleGroupId
    )
    {
        return new MuscleGroup(id, name, parentMuscleGroupId);
    }
}
