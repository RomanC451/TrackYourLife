using TrackYourLife.Modules.Trainings.Domain.Features.MuscleGroups;
using TrackYourLife.SharedLib.Domain.Ids;
using Xunit;

namespace TrackYourLife.Modules.Trainings.Domain.UnitTests.Features.MuscleGroups;

public class MuscleGroupIdTests
{
    [Fact]
    public void MuscleGroupId_ShouldInheritFromStronglyTypedGuid()
    {
        var type = typeof(MuscleGroupId);
        var baseType = typeof(StronglyTypedGuid<MuscleGroupId>);

        type.Should().BeAssignableTo(baseType);
    }

    [Fact]
    public void MuscleGroupId_ShouldBeRecord()
    {
        var type = typeof(MuscleGroupId);

        type.IsClass.Should().BeTrue();
        type.IsSealed.Should().BeTrue();
    }

    [Fact]
    public void Constructor_WithGuid_ShouldSetValue()
    {
        var guid = Guid.NewGuid();

        var id = new MuscleGroupId(guid);

        id.Value.Should().Be(guid);
    }

    [Fact]
    public void Constructor_Default_ShouldCreateEmptyGuid()
    {
        var id = new MuscleGroupId();

        id.Value.Should().Be(Guid.Empty);
    }

    [Theory]
    [InlineData("12345678-1234-1234-1234-123456789012", true)]
    [InlineData("invalid-guid", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void TryParse_WithValidInput_ShouldReturnCorrectResult(string? input, bool expectedSuccess)
    {
        var result = MuscleGroupId.TryParse(input, out var output);

        result.Should().Be(expectedSuccess);
        if (expectedSuccess)
        {
            output.Should().NotBeNull();
            output!.Value.ToString().Should().Be(input);
        }
        else
        {
            output.Should().BeNull();
        }
    }

    [Fact]
    public void ToString_ShouldReturnGuidString()
    {
        var guid = Guid.NewGuid();
        var id = new MuscleGroupId(guid);

        var result = id.ToString();

        result.Should().Be(guid.ToString());
    }

    [Fact]
    public void MuscleGroupId_ShouldBeEquatable()
    {
        var guid = Guid.NewGuid();
        var id1 = new MuscleGroupId(guid);
        var id2 = new MuscleGroupId(guid);
        var id3 = new MuscleGroupId(Guid.NewGuid());

        id1.Should().Be(id2);
        id1.Should().NotBe(id3);
        id1.GetHashCode().Should().Be(id2.GetHashCode());
    }
}
