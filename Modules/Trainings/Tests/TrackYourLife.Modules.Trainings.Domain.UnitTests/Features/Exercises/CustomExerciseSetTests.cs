using FluentAssertions;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using Xunit;

namespace TrackYourLife.Modules.Trainings.Domain.UnitTests.Features.Exercises;

public class CustomExerciseSetTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldSetProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "Rope Climbing";
        var orderIndex = 1;
        var customValue = "3";
        var customUnit = "ascents";
        var restTimeSeconds = 180;

        // Act
        var exerciseSet = new CustomExerciseSet(
            id,
            name,
            orderIndex,
            customValue,
            customUnit,
            restTimeSeconds
        );

        // Assert
        exerciseSet.Id.Should().Be(id);
        exerciseSet.Name.Should().Be(name);
        exerciseSet.OrderIndex.Should().Be(orderIndex);
        exerciseSet.CustomValue.Should().Be(customValue);
        exerciseSet.CustomUnit.Should().Be(customUnit);
        exerciseSet.RestTimeSeconds.Should().Be(restTimeSeconds);
    }

    [Fact]
    public void Update_WithValidParameters_ShouldUpdateProperties()
    {
        // Arrange
        var exerciseSet = new CustomExerciseSet(Guid.NewGuid(), "Rope Climbing", 1, "3", "ascents");
        var newCustomValue = "5";
        var newCustomUnit = "reps";
        var newRestTimeSeconds = 300;

        // Act
        var result = exerciseSet.Update(newCustomValue, newCustomUnit, newRestTimeSeconds);

        // Assert
        result.IsSuccess.Should().BeTrue();
        exerciseSet.CustomValue.Should().Be(newCustomValue);
        exerciseSet.CustomUnit.Should().Be(newCustomUnit);
        exerciseSet.RestTimeSeconds.Should().Be(newRestTimeSeconds);
    }

    [Fact]
    public void Update_WithEmptyCustomValue_ShouldReturnFailure()
    {
        // Arrange
        var exerciseSet = new CustomExerciseSet(Guid.NewGuid(), "Rope Climbing", 1, "3", "ascents");

        // Act
        var result = exerciseSet.Update(string.Empty, "reps");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }

    [Fact]
    public void Update_WithEmptyCustomUnit_ShouldReturnFailure()
    {
        // Arrange
        var exerciseSet = new CustomExerciseSet(Guid.NewGuid(), "Rope Climbing", 1, "3", "ascents");

        // Act
        var result = exerciseSet.Update("5", string.Empty);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }

    [Fact]
    public void Update_WithNullCustomValue_ShouldReturnFailure()
    {
        // Arrange
        var exerciseSet = new CustomExerciseSet(Guid.NewGuid(), "Rope Climbing", 1, "3", "ascents");

        // Act
        var result = exerciseSet.Update(null!, "reps");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }

    [Fact]
    public void Update_WithNullCustomUnit_ShouldReturnFailure()
    {
        // Arrange
        var exerciseSet = new CustomExerciseSet(Guid.NewGuid(), "Rope Climbing", 1, "3", "ascents");

        // Act
        var result = exerciseSet.Update("5", null!);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }

    [Fact]
    public void GetDisplayValue_ShouldReturnCorrectFormat()
    {
        // Arrange
        var exerciseSet = new CustomExerciseSet(Guid.NewGuid(), "Rope Climbing", 1, "3", "ascents");

        // Act
        var displayValue = exerciseSet.GetDisplayValue();

        // Assert
        displayValue.Should().Be("3 ascents");
    }

    [Fact]
    public void GetUnit_ShouldReturnCustomUnit()
    {
        // Arrange
        var exerciseSet = new CustomExerciseSet(Guid.NewGuid(), "Rope Climbing", 1, "3", "ascents");

        // Act
        var unit = exerciseSet.GetUnit();

        // Assert
        unit.Should().Be("ascents");
    }
}
