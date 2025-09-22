using FluentAssertions;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.SharedLib.Domain.Ids;
using Xunit;

namespace TrackYourLife.Modules.Trainings.Domain.UnitTests.Features.Exercises;

public class ExerciseIdTests
{
    [Fact]
    public void ExerciseId_ShouldInheritFromStronglyTypedGuid()
    {
        // Arrange & Act
        var exerciseIdType = typeof(ExerciseId);
        var baseType = typeof(StronglyTypedGuid<ExerciseId>);

        // Assert
        exerciseIdType.Should().BeAssignableTo(baseType);
    }

    [Fact]
    public void ExerciseId_ShouldBeRecord()
    {
        // Arrange & Act
        var exerciseIdType = typeof(ExerciseId);

        // Assert
        exerciseIdType.IsClass.Should().BeTrue();
        exerciseIdType.IsSealed.Should().BeTrue();
    }

    [Fact]
    public void Constructor_WithGuid_ShouldSetValue()
    {
        // Arrange
        var guid = Guid.NewGuid();

        // Act
        var exerciseId = new ExerciseId(guid);

        // Assert
        exerciseId.Value.Should().Be(guid);
    }

    [Fact]
    public void Constructor_Default_ShouldCreateEmptyGuid()
    {
        // Act
        var exerciseId = new ExerciseId();

        // Assert
        exerciseId.Value.Should().Be(Guid.Empty);
    }

    [Theory]
    [InlineData("12345678-1234-1234-1234-123456789012", true)]
    [InlineData("invalid-guid", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void TryParse_WithValidInput_ShouldReturnCorrectResult(
        string? input,
        bool expectedSuccess
    )
    {
        // Act
        var result = ExerciseId.TryParse(input, out var output);

        // Assert
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
        // Arrange
        var guid = Guid.NewGuid();
        var exerciseId = new ExerciseId(guid);

        // Act
        var result = exerciseId.ToString();

        // Assert
        result.Should().Be(guid.ToString());
    }

    [Fact]
    public void ExerciseId_ShouldBeEquatable()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var exerciseId1 = new ExerciseId(guid);
        var exerciseId2 = new ExerciseId(guid);
        var exerciseId3 = new ExerciseId(Guid.NewGuid());

        // Assert
        exerciseId1.Should().Be(exerciseId2);
        exerciseId1.Should().NotBe(exerciseId3);
        exerciseId1.GetHashCode().Should().Be(exerciseId2.GetHashCode());
    }
}
