using FluentAssertions;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using Xunit;

namespace TrackYourLife.Modules.Trainings.Domain.UnitTests.Features.ExercisesHistories;

public class ExerciseSetChangeTests
{
    [Fact]
    public void ExerciseSetChange_ShouldBeClass()
    {
        // Arrange & Act
        var exerciseSetChangeType = typeof(ExerciseSetChange);

        // Assert
        exerciseSetChangeType.IsClass.Should().BeTrue();
        exerciseSetChangeType.IsSealed.Should().BeTrue();
    }

    [Fact]
    public void Constructor_Default_ShouldInitializeProperties()
    {
        // Act
        var exerciseSetChange = new ExerciseSetChange();

        // Assert
        exerciseSetChange.SetId.Should().Be(Guid.Empty);
        exerciseSetChange.WeightChange.Should().Be(0);
        exerciseSetChange.RepsChange.Should().Be(0);
    }

    [Fact]
    public void Properties_ShouldBeSettable()
    {
        // Arrange
        var setId = Guid.NewGuid();
        var weightChange = 5.5f;
        var repsChange = 2;

        // Act
        var exerciseSetChange = new ExerciseSetChange
        {
            SetId = setId,
            WeightChange = weightChange,
            RepsChange = repsChange,
        };

        // Assert
        exerciseSetChange.SetId.Should().Be(setId);
        exerciseSetChange.WeightChange.Should().Be(weightChange);
        exerciseSetChange.RepsChange.Should().Be(repsChange);
    }

    [Theory]
    [InlineData(0.0f, 0)]
    [InlineData(5.5f, 2)]
    [InlineData(-2.5f, -1)]
    [InlineData(100.0f, 50)]
    public void Properties_ShouldAcceptVariousValues(float weightChange, int repsChange)
    {
        // Arrange
        var setId = Guid.NewGuid();

        // Act
        var exerciseSetChange = new ExerciseSetChange
        {
            SetId = setId,
            WeightChange = weightChange,
            RepsChange = repsChange,
        };

        // Assert
        exerciseSetChange.SetId.Should().Be(setId);
        exerciseSetChange.WeightChange.Should().Be(weightChange);
        exerciseSetChange.RepsChange.Should().Be(repsChange);
    }

    [Fact]
    public void ExerciseSetChange_ShouldBeMutable()
    {
        // Arrange
        var exerciseSetChange = new ExerciseSetChange
        {
            SetId = Guid.NewGuid(),
            WeightChange = 5.0f,
            RepsChange = 2,
        };

        var newSetId = Guid.NewGuid();
        var newWeightChange = 10.0f;
        var newRepsChange = 5;

        // Act
        exerciseSetChange.SetId = newSetId;
        exerciseSetChange.WeightChange = newWeightChange;
        exerciseSetChange.RepsChange = newRepsChange;

        // Assert
        exerciseSetChange.SetId.Should().Be(newSetId);
        exerciseSetChange.WeightChange.Should().Be(newWeightChange);
        exerciseSetChange.RepsChange.Should().Be(newRepsChange);
    }
}

