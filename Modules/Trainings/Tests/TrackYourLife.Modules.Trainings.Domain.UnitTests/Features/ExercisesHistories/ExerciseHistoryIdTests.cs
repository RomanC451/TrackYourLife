using FluentAssertions;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.SharedLib.Domain.Ids;
using Xunit;

namespace TrackYourLife.Modules.Trainings.Domain.UnitTests.Features.ExercisesHistories;

public class ExerciseHistoryIdTests
{
    [Fact]
    public void ExerciseHistoryId_ShouldInheritFromStronglyTypedGuid()
    {
        // Arrange & Act
        var exerciseHistoryIdType = typeof(ExerciseHistoryId);
        var baseType = typeof(StronglyTypedGuid<ExerciseHistoryId>);

        // Assert
        exerciseHistoryIdType.Should().BeAssignableTo(baseType);
    }

    [Fact]
    public void ExerciseHistoryId_ShouldBeRecord()
    {
        // Arrange & Act
        var exerciseHistoryIdType = typeof(ExerciseHistoryId);

        // Assert
        exerciseHistoryIdType.IsClass.Should().BeTrue();
        exerciseHistoryIdType.IsSealed.Should().BeTrue();
    }

    [Fact]
    public void Constructor_WithGuid_ShouldSetValue()
    {
        // Arrange
        var guid = Guid.NewGuid();

        // Act
        var exerciseHistoryId = new ExerciseHistoryId(guid);

        // Assert
        exerciseHistoryId.Value.Should().Be(guid);
    }

    [Fact]
    public void Constructor_Default_ShouldCreateEmptyGuid()
    {
        // Act
        var exerciseHistoryId = new ExerciseHistoryId();

        // Assert
        exerciseHistoryId.Value.Should().Be(Guid.Empty);
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
        var result = ExerciseHistoryId.TryParse(input, out var output);

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
        var exerciseHistoryId = new ExerciseHistoryId(guid);

        // Act
        var result = exerciseHistoryId.ToString();

        // Assert
        result.Should().Be(guid.ToString());
    }

    [Fact]
    public void ExerciseHistoryId_ShouldBeEquatable()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var exerciseHistoryId1 = new ExerciseHistoryId(guid);
        var exerciseHistoryId2 = new ExerciseHistoryId(guid);
        var exerciseHistoryId3 = new ExerciseHistoryId(Guid.NewGuid());

        // Assert
        exerciseHistoryId1.Should().Be(exerciseHistoryId2);
        exerciseHistoryId1.Should().NotBe(exerciseHistoryId3);
        exerciseHistoryId1.GetHashCode().Should().Be(exerciseHistoryId2.GetHashCode());
    }
}

