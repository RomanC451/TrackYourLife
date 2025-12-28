using FluentAssertions;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using Xunit;

namespace TrackYourLife.Modules.Trainings.Domain.UnitTests.Features.Exercises;

public class ExerciseSetTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldReturnSuccess()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "Bench Press";
        var orderIndex = 1;
        var count1 = 10f;
        var unit1 = "reps";
        var count2 = 50.5f;
        var unit2 = "kg";
        var restTimeSeconds = 120;

        // Act
        var result = ExerciseSet.Create(
            id,
            name,
            orderIndex,
            count1,
            unit1,
            count2,
            unit2,
            restTimeSeconds
        );

        // Assert
        result.IsSuccess.Should().BeTrue();
        var exerciseSet = result.Value;
        exerciseSet.Id.Should().Be(id);
        exerciseSet.Name.Should().Be(name);
        exerciseSet.OrderIndex.Should().Be(orderIndex);
        exerciseSet.Count1.Should().Be(count1);
        exerciseSet.Unit1.Should().Be(unit1);
        exerciseSet.Count2.Should().Be(count2);
        exerciseSet.Unit2.Should().Be(unit2);
        exerciseSet.RestTimeSeconds.Should().Be(restTimeSeconds);
    }

    [Fact]
    public void Create_WithOnlyCount1AndUnit1_ShouldReturnSuccess()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "Plank";
        var orderIndex = 1;
        var count1 = 60f;
        var unit1 = "seconds";

        // Act
        var result = ExerciseSet.Create(id, name, orderIndex, count1, unit1);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var exerciseSet = result.Value;
        exerciseSet.Count1.Should().Be(count1);
        exerciseSet.Unit1.Should().Be(unit1);
        exerciseSet.Count2.Should().BeNull();
        exerciseSet.Unit2.Should().BeNull();
    }

    [Fact]
    public void Create_WithEmptyId_ShouldReturnFailure()
    {
        // Arrange
        var emptyId = Guid.Empty;

        // Act
        var result = ExerciseSet.Create(emptyId, "Test", 0, 10, "reps");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithInvalidName_ShouldReturnFailure(string? name)
    {
        // Act
        var result = ExerciseSet.Create(Guid.NewGuid(), name!, 0, 10, "reps");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }

    [Fact]
    public void Create_WithNegativeOrderIndex_ShouldReturnFailure()
    {
        // Act
        var result = ExerciseSet.Create(Guid.NewGuid(), "Test", -1, 10, "reps");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }

    [Fact]
    public void Create_WithNegativeCount1_ShouldReturnFailure()
    {
        // Act
        var result = ExerciseSet.Create(Guid.NewGuid(), "Test", 0, -10, "reps");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithInvalidUnit1_ShouldReturnFailure(string? unit1)
    {
        // Act
        var result = ExerciseSet.Create(Guid.NewGuid(), "Test", 0, 10, unit1!);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }

    [Fact]
    public void Create_WithCount2ButNoUnit2_ShouldReturnFailure()
    {
        // Act
        var result = ExerciseSet.Create(Guid.NewGuid(), "Test", 0, 10, "reps", 50.0f, null);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }

    [Fact]
    public void Create_WithUnit2ButNoCount2_ShouldReturnFailure()
    {
        // Act
        var result = ExerciseSet.Create(Guid.NewGuid(), "Test", 0, 10, "reps", null, "kg");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }

    [Fact]
    public void Create_WithNegativeCount2_ShouldReturnFailure()
    {
        // Act
        var result = ExerciseSet.Create(Guid.NewGuid(), "Test", 0, 10, "reps", -50.0f, "kg");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }
}
