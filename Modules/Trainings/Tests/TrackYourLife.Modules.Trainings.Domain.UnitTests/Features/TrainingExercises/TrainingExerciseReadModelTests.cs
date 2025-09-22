using FluentAssertions;
using TrackYourLife.Modules.Trainings.Domain.Core;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.TrainingExercises;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.SharedLib.Domain.Ids;
using Xunit;

namespace TrackYourLife.Modules.Trainings.Domain.UnitTests.Features.TrainingExercises;

public class TrainingExerciseReadModelTests
{
    [Fact]
    public void TrainingExerciseReadModel_ShouldBeRecord()
    {
        // Arrange & Act
        var readModelType = typeof(TrainingExerciseReadModel);

        // Assert
        readModelType.IsClass.Should().BeTrue();
        readModelType.IsSealed.Should().BeTrue();
    }

    [Fact]
    public void Constructor_WithValidParameters_ShouldSetProperties()
    {
        // Arrange
        var trainingId = TrainingId.Create(Guid.NewGuid());
        var exerciseId = ExerciseId.Create(Guid.NewGuid());
        var orderIndex = 2;

        // Act
        var readModel = new TrainingExerciseReadModel(trainingId, exerciseId, orderIndex);

        // Assert
        readModel.TrainingId.Should().Be(trainingId);
        readModel.ExerciseId.Should().Be(exerciseId);
        readModel.OrderIndex.Should().Be(orderIndex);
    }

    [Fact]
    public void Exercise_ShouldBeSettable()
    {
        // Arrange
        var readModel = new TrainingExerciseReadModel(
            TrainingId.Create(Guid.NewGuid()),
            ExerciseId.Create(Guid.NewGuid()),
            0
        );

        var exercise = new ExerciseReadModel(
            ExerciseId.Create(Guid.NewGuid()),
            UserId.Create(Guid.NewGuid()),
            "Test Exercise",
            new List<string> { "Chest" },
            Difficulty.Easy,
            null,
            null,
            null,
            null,
            DateTime.UtcNow,
            null
        );

        // Act
        readModel.Exercise = exercise;

        // Assert
        readModel.Exercise.Should().Be(exercise);
    }

    [Fact]
    public void Exercise_ShouldBeNullByDefault()
    {
        // Arrange & Act
        var readModel = new TrainingExerciseReadModel(
            TrainingId.Create(Guid.NewGuid()),
            ExerciseId.Create(Guid.NewGuid()),
            0
        );

        // Assert
        readModel.Exercise.Should().BeNull();
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(10, 10)]
    [InlineData(100, 100)]
    public void OrderIndex_ShouldAcceptVariousValues(int orderIndex, int expectedValue)
    {
        // Arrange & Act
        var readModel = new TrainingExerciseReadModel(
            TrainingId.Create(Guid.NewGuid()),
            ExerciseId.Create(Guid.NewGuid()),
            orderIndex
        );

        // Assert
        readModel.OrderIndex.Should().Be(expectedValue);
    }

    [Fact]
    public void TrainingExerciseReadModel_ShouldBeEquatable()
    {
        // Arrange
        var trainingId = TrainingId.Create(Guid.NewGuid());
        var exerciseId = ExerciseId.Create(Guid.NewGuid());
        var orderIndex = 5;

        // Act
        var readModel1 = new TrainingExerciseReadModel(trainingId, exerciseId, orderIndex);
        var readModel2 = new TrainingExerciseReadModel(trainingId, exerciseId, orderIndex);
        var readModel3 = new TrainingExerciseReadModel(
            TrainingId.Create(Guid.NewGuid()),
            exerciseId,
            orderIndex
        );

        // Assert
        readModel1.Should().Be(readModel2);
        readModel1.Should().NotBe(readModel3);
        readModel1.GetHashCode().Should().Be(readModel2.GetHashCode());
    }

    [Fact]
    public void TrainingExerciseReadModel_WithDifferentOrderIndex_ShouldBeDifferent()
    {
        // Arrange
        var trainingId = TrainingId.Create(Guid.NewGuid());
        var exerciseId = ExerciseId.Create(Guid.NewGuid());

        // Act
        var readModel1 = new TrainingExerciseReadModel(trainingId, exerciseId, 0);
        var readModel2 = new TrainingExerciseReadModel(trainingId, exerciseId, 1);

        // Assert
        readModel1.TrainingId.Should().Be(readModel2.TrainingId);
        readModel1.ExerciseId.Should().Be(readModel2.ExerciseId);
        readModel1.OrderIndex.Should().NotBe(readModel2.OrderIndex);
    }

    [Fact]
    public void TrainingExerciseReadModel_WithDifferentExerciseId_ShouldBeDifferent()
    {
        // Arrange
        var trainingId = TrainingId.Create(Guid.NewGuid());
        var orderIndex = 0;

        // Act
        var readModel1 = new TrainingExerciseReadModel(
            trainingId,
            ExerciseId.Create(Guid.NewGuid()),
            orderIndex
        );
        var readModel2 = new TrainingExerciseReadModel(
            trainingId,
            ExerciseId.Create(Guid.NewGuid()),
            orderIndex
        );

        // Assert
        readModel1.TrainingId.Should().Be(readModel2.TrainingId);
        readModel1.ExerciseId.Should().NotBe(readModel2.ExerciseId);
        readModel1.OrderIndex.Should().Be(readModel2.OrderIndex);
    }

    [Fact]
    public void TrainingExerciseReadModel_WithDifferentTrainingId_ShouldBeDifferent()
    {
        // Arrange
        var exerciseId = ExerciseId.Create(Guid.NewGuid());
        var orderIndex = 0;

        // Act
        var readModel1 = new TrainingExerciseReadModel(
            TrainingId.Create(Guid.NewGuid()),
            exerciseId,
            orderIndex
        );
        var readModel2 = new TrainingExerciseReadModel(
            TrainingId.Create(Guid.NewGuid()),
            exerciseId,
            orderIndex
        );

        // Assert
        readModel1.TrainingId.Should().NotBe(readModel2.TrainingId);
        readModel1.ExerciseId.Should().Be(readModel2.ExerciseId);
        readModel1.OrderIndex.Should().Be(readModel2.OrderIndex);
    }
}
