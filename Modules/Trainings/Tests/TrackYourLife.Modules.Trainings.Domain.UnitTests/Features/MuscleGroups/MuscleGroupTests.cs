using TrackYourLife.Modules.Trainings.Domain.Features.MuscleGroups;
using Xunit;

namespace TrackYourLife.Modules.Trainings.Domain.UnitTests.Features.MuscleGroups;

public class MuscleGroupTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldReturnMuscleGroup()
    {
        var id = new MuscleGroupId(Guid.NewGuid());
        var name = "Chest";
        MuscleGroupId? parentId = null;

        var result = MuscleGroup.Create(id, name, parentId);

        result.Should().NotBeNull();
        result.Id.Should().Be(id);
        result.Name.Should().Be(name);
        result.ParentMuscleGroupId.Should().BeNull();
    }

    [Fact]
    public void Create_WithParentId_ShouldSetParentMuscleGroupId()
    {
        var id = new MuscleGroupId(Guid.NewGuid());
        var parentId = new MuscleGroupId(Guid.NewGuid());
        var name = "Upper Chest";

        var result = MuscleGroup.Create(id, name, parentId);

        result.Should().NotBeNull();
        result.Id.Should().Be(id);
        result.Name.Should().Be(name);
        result.ParentMuscleGroupId.Should().Be(parentId);
    }

    [Fact]
    public void Create_WithSameIdAndName_ShouldProduceEqualInstances()
    {
        var id = new MuscleGroupId(Guid.NewGuid());
        var name = "Triceps";

        var result1 = MuscleGroup.Create(id, name, null);
        var result2 = MuscleGroup.Create(id, name, null);

        result1.Id.Should().Be(result2.Id);
        result1.Name.Should().Be(result2.Name);
    }
}
